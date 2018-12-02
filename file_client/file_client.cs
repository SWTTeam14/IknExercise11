using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
    public class file_client
    {

        private const int BUFSIZE = 1000;
        private const string APP = "FILE_CLIENT";

        private file_client(String[] args)
        {
            ITransport trans = new Transport(BUFSIZE, APP);

            byte[] bytesToSend = new byte[BUFSIZE];
            bytesToSend[0] = (byte)'A';
            bytesToSend[1] = (byte)'X';
            bytesToSend[2] = (byte)'B';
            bytesToSend[3] = (byte)'Y';

            trans.Send(bytesToSend, 4);
            // TO DO Your own code
        }


        private void receiveFile(String fileName, ITransport transport)
        {
            // TO DO Your own code
        }


        public static void Main(string[] args)
        {
            file_client fc = new file_client(args);
        }
    }
}