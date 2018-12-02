using System;
namespace Transportlaget
{
	public interface ITransport
	{
		void Send(byte[] buf, int size);
        int Receive(ref byte[] buf);
	}
}
