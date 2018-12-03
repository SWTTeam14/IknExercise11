using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;


namespace Linklaget
{

    public class Link
    {

        const byte DELIMITER = (byte)'A';

        private byte[] buffer;

        SerialPort serialPort;


        public Link(int BUFSIZE, string APP)
        {
            // Create a new SerialPort object with default settings.

            serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);

            if (!serialPort.IsOpen)
                serialPort.Open();

            buffer = new byte[(BUFSIZE * 2)];

            // Uncomment the next line to use timeout
            //serialPort.ReadTimeout = 500;

            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        /// <summary>
        /// Send the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public void Send(byte[] buf, int size)
        {
			// TO DO Your own code

			int index= 0;
			buffer[index] = DELIMITER;
			++index;

            for (int i = 0; i < size; i++)
            {
                if (buf[i] == (byte)'A')
                {
					buffer[index] = (byte)'B';
					++index;
					buffer[index] = (byte)'C';
					++index;
                }
                else if (buf[i] == (byte)'B')
                {
					buffer[index] = (byte)'B';
					++index;
					buffer[index] = (byte)'D';
					++index;
                }
                else
                {
					buffer[index] = buf[i];
					++index;
                }

            }

			buffer[index] = (byte)'A';
			++index;
            
            serialPort.Write(buffer, 0, index);         
        }
      
        public int Receive(ref byte[] buf)
        {
            List<byte> byteList = new List<byte>();
            List<byte> finalByteList = new List<byte>();
            int readbyte = 0;
			var checkOnA = (int)Convert.ToByte('A');

			while (readbyte != checkOnA)
            {
                readbyte = serialPort.ReadByte();
            }

            readbyte = 0;

			while (readbyte != checkOnA)
            {
                readbyte = serialPort.ReadByte();

				if (readbyte == (byte)'B')
                    {
    					if(readbyte == (byte)'C')
    						finalByteList.Add((byte)'A');
                    }
				else if (readbyte == (byte)'B')
                    {
    					if(readbyte == (byte)'D')
                            finalByteList.Add((byte)'B');
                    }
				else if(readbyte == (byte)'A')
    				{
    				    Console.WriteLine("End of array...");
				}
                else
                    {
					    finalByteList.Add((byte)readbyte);
                    }            
			}

            buf = finalByteList.OfType<byte>().ToArray();
            
            return buf.Length;
        }

        public byte[] GetBuffer()
		{
			return buffer; 
		}
    }
}