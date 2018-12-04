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

			if (recvSize == (int)TransSize.ACKSIZE)
			{
				dataReceived = false;
				if (!checksum.checkChecksum(buffer, (int)TransSize.ACKSIZE) ||
				  buffer[(int)TransCHKSUM.SEQNO] != seqNo ||
				  buffer[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				{
					return false;
				}
				seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
			}

			return true;
		}

		private void sendAck(bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf[(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer[(int)TransCHKSUM.SEQNO] : (byte)(buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf[(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum(ref ackBuf, (int)TransSize.ACKSIZE);
			link.Send(ackBuf, (int)TransSize.ACKSIZE);
		}


		public void Send(byte[] buf, int size)
		{
            buffer = new byte[size+4];
			//Copy data
			int i = 0;
            for (int j = 4; j < size+4; j++)
			{
                buffer[j] = buf[i];
                i++;
			}

            //Write header - four bytes
            buffer[(int)TransCHKSUM.SEQNO] = seqNo;
            buffer[(int)TransCHKSUM.TYPE] = (int)TransType.DATA;
            checksum.calcChecksum(ref buffer, size + 4);
            //Send buffer
			link.Send(buffer, size + 4);
            
            int _numberOfBadMessages = 0;
			while (!receiveAck() && _numberOfBadMessages < 4)
			{
				link.Send(buffer, size + 4);
				_numberOfBadMessages++;
                
				if (_numberOfBadMessages == 4)
				{
					Console.WriteLine("Failed 5 times, terminating...");
					return;
				}
			}
		}

		public int Receive(ref byte[] buf)
		{
			bool _isSeqNoDifferent = false;
			bool _isCheckSumOk = false;
			int len = -1;
                     
			do
			{
				len = link.Receive(ref buffer);
            
				_isCheckSumOk = checksum.checkChecksum(buffer, len);

				if (buffer[(int)TransCHKSUM.SEQNO] != old_seqNo)
					_isSeqNoDifferent = true;

				if (!_isCheckSumOk || !_isSeqNoDifferent)
					sendAck(false);
				else
					sendAck(true);

			} while (!_isSeqNoDifferent && !_isCheckSumOk);

			old_seqNo = buffer[(int)TransCHKSUM.SEQNO];

			buf = buffer.Skip(4).ToArray();
            return buf.Length;
		}
	}
}