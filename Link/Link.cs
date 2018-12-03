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

        public void Send(byte[] buf, int size)
        {
<<<<<<< HEAD
			// TO DO Your own code

			int index= 0;
			buffer[index] = DELIMITER;
			++index;
=======
            List<Byte> byteList = new List<Byte>();
            byteList.Add(DELIMITER);
>>>>>>> 96c34c9e9a1fa7e9ec8ae77634ea05a1c6a03f93

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
<<<<<<< HEAD

			buffer[index] = (byte)'A';
			++index;
            
            serialPort.Write(buffer, 0, index);         
=======
            byteList.Add(DELIMITER);
            buffer = byteList.OfType<byte>().ToArray();
            serialPort.Write(buffer, 0, buffer.Length);
>>>>>>> 96c34c9e9a1fa7e9ec8ae77634ea05a1c6a03f93
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
<<<<<<< HEAD
=======
                byteList.Add((byte)readbyte);				
            }
>>>>>>> 96c34c9e9a1fa7e9ec8ae77634ea05a1c6a03f93

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
<<<<<<< HEAD
                    {
					    finalByteList.Add((byte)readbyte);
                    }            
			}

            buf = finalByteList.OfType<byte>().ToArray();
            
=======
                {
					finalByteList.Add(byteList[i]);
                }
            }
            buf = finalByteList.OfType<byte>().ToArray();
>>>>>>> 96c34c9e9a1fa7e9ec8ae77634ea05a1c6a03f93
            return buf.Length;
        }
        public byte[] GetBuffer()
		{
			return buffer; 
		}
    }
}