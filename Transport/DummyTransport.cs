using System;
using Linklaget;


namespace Transportlaget
{
	public class DummyTransport : ITransport 
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

	
		public DummyTransport (int BUFSIZE, string APP)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
			dataReceived = false;
		}

		private bool receiveAck()
		{
			recvSize = link.Receive(ref buffer);
			dataReceived = true;

			if (recvSize == (int)TransSize.ACKSIZE) {
				dataReceived = false;
				if (!checksum.checkChecksum (buffer, (int)TransSize.ACKSIZE) ||
				  buffer [(int)TransCHKSUM.SEQNO] != seqNo ||
				  buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				{
					return false;
				}
				seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
			}
 
			return true;
		}


		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);
			link.Send(ackBuf, (int)TransSize.ACKSIZE);
		}

	
		public void Send(byte[] buf, int size)
		{
			link.Send(buf, size);

        }
        

		public int Receive (ref byte[] buf)
		{
			int size = link.Receive(ref buf);
            return size;
		}
	}
}