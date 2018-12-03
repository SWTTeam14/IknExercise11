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
            List<Byte> byteList = new List<Byte>();
            byteList.Add(DELIMITER);

            for (int i = 0; i < size; i++)
            {
                if (buf[i] == (byte)'A')
                {
                    byteList.Add((byte)'B');
                    byteList.Add((byte)'C');
                }
                else if (buf[i] == (byte)'B')
                {
                    byteList.Add((byte)'B');
                    byteList.Add((byte)'D');
                }
                else
                {
                    byteList.Add(buf[i]);
                }
            }
            byteList.Add(DELIMITER);
            buffer = byteList.OfType<byte>().ToArray();
            serialPort.Write(buffer, 0, buffer.Length);
        }
      
        public int Receive(ref byte[] buf)
        {
            List<byte> byteList = new List<byte>();
            List<byte> finalByteList = new List<byte>();
            int readbyte = 0;

            while (readbyte != 'A')
            {
                readbyte = serialPort.ReadByte();
				byteList.Add((byte)readbyte);
            }
            readbyte = 0;

            while (readbyte != 'A')
            {
                readbyte = serialPort.ReadByte();
                byteList.Add((byte)readbyte);				
            }

            for (int i = 1; i < byteList.Count; i++)
            {
                if (byteList[i] == (byte)'B' && byteList[i + 1] == (byte)'C')
                {
                    finalByteList.Add((byte)'A');
					i++;
                }
				else if (byteList[i] == (byte)'B' && byteList[i + 1] == (byte)'D')
                {
                    finalByteList.Add((byte)'B');
					i++;
                }
				else if(byteList[i] == (byte)'A')
				{
					i++;
				}            
                else
                {
					finalByteList.Add(byteList[i]);
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