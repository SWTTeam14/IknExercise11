using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
    class file_server
    {
        /// <summary>
        /// The BUFSIZE
        /// </summary>
        private const int BUFSIZE = 1000;
        private const string APP = "FILE_SERVER";

        private file_server()
        {
            Transport trans = new Transport(BUFSIZE, APP);
            byte[] bytesToReceive = new byte[BUFSIZE];

            trans.Receive(ref bytesToReceive);
			char[] c = System.Text.Encoding.ASCII.GetChars(bytesToReceive);
            for (int i = 0; i < 4; i++)
            {
				
                Console.WriteLine("Received byte: " + c[i]);
            }

            // TO DO Your own code
        }

        private void sendFile(String fileName, long fileSize, Transport transport)
        {
            // TO DO Your own code
        }


        public static void Main(string[] args)
        {
            file_server fs = new file_server();
        }
    }
}