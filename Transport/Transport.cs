using System;
using Linklaget;


namespace Transportlaget
{
    public class Transport
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

	
		public Transport (int BUFSIZE, string APP)
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
			// TO DO Your own code

			buffer[(int)TransCHKSUM.SEQNO] = seqNo;
			buffer[(int)TransCHKSUM.TYPE] = 0;

            for (int i = 0; i < size; i++)
			{
				buffer[i+4] = buf[i];
			}
			checksum.calcChecksum(ref buffer, buffer.Length);


			link.Send(buffer, size);

			int numberOfBadMessages = 0;
			while(numberOfBadMessages < 4 && receiveAck() == false){
				numberOfBadMessages++;
				link.Send(buffer, size);
                Console.WriteLine(numberOfBadMessages);
				if(numberOfBadMessages == 4){
					Console.WriteLine("Failed 5 times, terminating...");
					return;
				}
			}

			if(seqNo == 0){
				seqNo = 1;
			} else if (seqNo == 1){
				seqNo = 0;
			}
            
		}
        

		public int Receive (ref byte[] buf)
		{
			// TO DO Your own code
            checksum.
		    int size = link.Receive(ref buf);
		    return size;
		}
	}
}