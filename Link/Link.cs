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
        private SerialPort serialPort;
        private FakeSerialPort fakeSerialPort;
        private bool _useFakeSerialPort;

        public Link(int BUFSIZE, string APP) : this(BUFSIZE, APP, false, new byte[] { })
        {
        }

        public Link(int BUFSIZE, string APP, bool useFakeSerialPort, byte[] fakeBuffer)
        {
            // Create a new SerialPort object with default settings.
            _useFakeSerialPort = useFakeSerialPort;
            if (useFakeSerialPort)
            {
                fakeSerialPort = new FakeSerialPort(fakeBuffer);
            }
            else
            {
                serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);

                if (!serialPort.IsOpen)
                    serialPort.Open();

                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
            }

            buffer = new byte[(BUFSIZE * 2)];
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
            List<byte> initialByteList = new List<byte>();
            List<byte> finalByteList = new List<byte>();
            int readbyte = 0;
            var checkOnA = (int)Convert.ToByte('A');

            //Skip all characters until start byte is received
            while (readbyte != checkOnA)
            {
                readbyte = _useFakeSerialPort ? fakeSerialPort.ReadByte() : serialPort.ReadByte();
            }

            readbyte = 0;

            //Read all bytes into a byte array
            while (readbyte != checkOnA)
            {
                readbyte = _useFakeSerialPort ? fakeSerialPort.ReadByte() : serialPort.ReadByte();
                if (readbyte != checkOnA)
                {
                    initialByteList.Add((byte)readbyte);
                }
            }
            buf = Decoder(initialByteList.ToArray());
            return buf.Length;
        }

        //Decode content of a frame - note this content is without delimiter. 
        public byte[] Decoder(byte[] input)
        {
            List<byte> finalByteList = new List<byte>();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == (byte)'B' && input[i + 1] == (byte)'C')
                {
                    finalByteList.Add((byte)'A');
                    i++;
                }
                else if (input[i] == (byte)'B' && input[i + 1] == (byte)'D')
                {
                    finalByteList.Add((byte)'B');
                    i++;
                }
                else
                {
                    finalByteList.Add(input[i]);
                }
            }

            return finalByteList.ToArray();
        }

        //Only made so it is possible to test Link layers send method
        public byte[] GetBuffer()
        {
            return buffer;
        }
    }
}