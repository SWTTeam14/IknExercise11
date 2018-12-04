using System;
using System.Linq;
using Linklaget;

namespace Transportlaget
{
    public class Transport : ITransport
    {
        private Link link;
        private Checksum checksum;
        private byte[] buffer;
        private byte seqNo;
        private byte old_seqNo;
        private int errorCount;
        private const int DEFAULT_SEQNO = 2;
        private bool dataReceived;
        private int recvSize = 0;

        public Transport(int BUFSIZE, string APP)
        {
            link = new Link(BUFSIZE + (int)TransSize.ACKSIZE, APP);
            checksum = new Checksum();
            buffer = new byte[BUFSIZE + (int)TransSize.ACKSIZE];
            seqNo = 0;
            old_seqNo = DEFAULT_SEQNO;
            errorCount = 0;
            dataReceived = false;
        }

        private bool receiveAck()
        {
            recvSize = link.Receive(ref buffer);
            dataReceived = true;

            //Note that receiveSize must be four - otherwise true i returned. This may be a bug.
            if (recvSize == (int)TransSize.ACKSIZE)
            {
                dataReceived = false;
                if (!checksum.checkChecksum(buffer, (int)TransSize.ACKSIZE) ||
                  buffer[(int)TransCHKSUM.SEQNO] != seqNo ||
                  buffer[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
                {
                    return false; //No valid ACK has been received
                }
                seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            }
            return true; //ACK received
        }

        private void sendAck(bool ackType)
        {
            byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
            ackBuf[(int)TransCHKSUM.SEQNO] = (byte)
                (ackType ? (byte)buffer[(int)TransCHKSUM.SEQNO] : (byte)(buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            ackBuf[(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
            checksum.calcChecksum(ref ackBuf, (int)TransSize.ACKSIZE);
            if (++errorCount == 3) // Simulate noise
            {
                ackBuf[1]++; // Important: Only spoil a checksum-field (ackBuf[0] or ackBuf[1])
                Console.WriteLine("Noise!byte #1 is spoiled in the third transmitted ACK-package");
            }
            link.Send(ackBuf, (int)TransSize.ACKSIZE);
        }


        public void Send(byte[] buf, int size)
        {
            buffer = new byte[size + 4];
            //Copy data
            for (int i = 0; i < size; i++)
            {
                buffer[i + 4] = buf[i];
            }

            //Write header - four bytes
            buffer[(int)TransCHKSUM.SEQNO] = seqNo;
            buffer[(int)TransCHKSUM.TYPE] = (int)TransType.DATA;
            checksum.calcChecksum(ref buffer, size + 4);
            if (++errorCount == 3) // Simulate noise
            {
                buffer[1]++; // Important: Only spoil a checksum-field (buffer[0] or buffer[1])
                Console.WriteLine("Noise!-byte #1 is spoiled in the third transmission");
            }
            //Send buffer
            Console.WriteLine("Send: seqNo = {0}, TransType = {1}, errorCount = {2}", seqNo, (int)TransType.DATA, errorCount);
            link.Send(buffer, size + 4);
            int numberOfTransmits = 1;
            while (!receiveAck())
            {
                Console.WriteLine("Retransmision. numberOfTransmits = {0}", numberOfTransmits);
                if (numberOfTransmits == 5)
                {
                    Console.WriteLine("Failed 5 times, terminating...");
                    return;
                }
                Console.WriteLine("Send: seqNo = {0}, TransType = {1}, errorCount = {2}", seqNo, (int)TransType.DATA, errorCount);
                link.Send(buffer, size + 4);
                numberOfTransmits++;
            }
            old_seqNo = DEFAULT_SEQNO;
        }

        public int Receive(ref byte[] buf)
        {
            bool isAllOk;
            int len;

            do
            {
                len = link.Receive(ref buffer);
                bool isCheckSumOk = checksum.checkChecksum(buffer, len);
                bool isSeqNoDifferent = (buffer[(int)TransCHKSUM.SEQNO] != old_seqNo);
                Console.WriteLine("Receive: seqNo = {0}, old_seqNo = {1}", buffer[(int)TransCHKSUM.SEQNO], old_seqNo);
                isAllOk = isCheckSumOk && isSeqNoDifferent;
                sendAck(isAllOk);
            } while (!isAllOk);

            old_seqNo = buffer[(int)TransCHKSUM.SEQNO];
            buf = buffer.Skip(4).ToArray();
            return buf.Length;
        }
    }
}