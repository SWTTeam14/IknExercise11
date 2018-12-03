using System;
using System.IO.Ports;
using System.Linq;

namespace Linklaget
{
    public class FakeSerialPort : SerialPort
    {
        private byte[] _readBuffer;

        public FakeSerialPort(byte[] readBuffer)
        {
            _readBuffer = readBuffer;
        }

        public new int ReadByte()
        {
            byte b = _readBuffer[0];
            _readBuffer = _readBuffer.Skip(1).Take(_readBuffer.Count()-1).ToArray();
            return b;
        }      
    }
}
