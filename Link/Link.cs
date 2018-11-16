using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					serialPort = new SerialPort("/dev/ttySn0",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					serialPort = new SerialPort("/dev/ttySn1",115200,Parity.None,8,StopBits.One);
				}
			#else
				serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			#endif
			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;

			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
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
		public void Send (byte[] buf, int size)
		{
	    	// TO DO Your own code

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

		/// <summary>

		public int Receive (ref byte[] buf)
		{
            
            

            while(buffer[0] != 'A'){
                // Do nothing
                serialPort.Read(buffer, 0, buffer.Length);
            }
            
            List<Byte> byteList = new List<byte>();

		    for (int i = 1; i < buffer.Length; i++)
		    {
		        if (buffer[i] == (byte)'B' && buffer[i+1] == (byte)'C')
		        {
                    byteList.Add((byte)'A');
		        }
                else if (buffer[i] == (byte)'B' && buffer[i + 1] == (byte)'D')
		        {
                    byteList.Add((byte)'B');
		        }
		        else
		        {
                    byteList.Add(buffer[i]);
		        }
		    }

		    buf = byteList.OfType<byte>().ToArray();
            
            
            return buf.Length;
		}
        
	}
}
