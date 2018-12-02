using System;
using System.Linq;
using System.Text;
using Linklaget;
using NUnit.Framework;

namespace LinkTest
{
	[TestFixture]
    public class LinkTest
    {
		private const int BUFSIZE = 1000;
        private const string APP = "TEST_SERVER";
          
		[Test]
        public void Send_Test1()
        {
            Link link = new Link(BUFSIZE, APP);
            byte[] bytesToSend = Encoding.ASCII.GetBytes("AXBY");
            link.Send(bytesToSend, bytesToSend.Length);
            byte[] bytesExpected = Encoding.ASCII.GetBytes("ABCXBDYA");

			byte[] bytesSend = link.GetBuffer();
			Assert.IsTrue(bytesExpected.SequenceEqual(bytesSend));
        }

		[Test]
        public void Send_Test2()
        {
            Link link = new Link(BUFSIZE, APP);
            byte[] bytesToSend = Encoding.ASCII.GetBytes("HXXABY");
            link.Send(bytesToSend, bytesToSend.Length);
            byte[] bytesExpected = Encoding.ASCII.GetBytes("AHXXBCBDYA");

            byte[] bytesSend = link.GetBuffer();
            Assert.IsTrue(bytesExpected.SequenceEqual(bytesSend));
        }

		[Test]
        public void Send_Test3()
        {
            Link link = new Link(BUFSIZE, APP);
            byte[] bytesToSend = Encoding.ASCII.GetBytes("MAABBY");
            link.Send(bytesToSend, bytesToSend.Length);
            byte[] bytesExpected = Encoding.ASCII.GetBytes("AMBCBCBDBDYA");

            byte[] bytesSend = link.GetBuffer();
            Assert.IsTrue(bytesExpected.SequenceEqual(bytesSend));
        }
    }
}
