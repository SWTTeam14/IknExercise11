using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
    public class File_client
    {
        private const int BUFSIZE = 1000;
        private const string APP = "FILE_CLIENT";
        
        private File_client(String[] args)
        {
            ITransport trans = new Transport(BUFSIZE, APP);
            ReceiveFile(args[0], trans);
        }

		private void ReceiveFile(String fileName, ITransport transport)
        {
			byte[] fileNameBuffer = Encoding.ASCII.GetBytes(fileName);
			transport.Send(fileNameBuffer, fileNameBuffer.Length);
			string newFileName = LIB.extractFileName(fileName);
            FileStream fs = File.Create(newFileName);
			byte[] buffer = new Byte[BUFSIZE];
			transport.Receive(ref buffer);
			long fileSize = BitConverter.ToInt64(buffer, 0);
            if (fileSize == 0)
			{
				throw new FileNotFoundException();
			}

            byte[] chunks = new byte[BUFSIZE];
            while (fileSize > 0)
            {
				int m = transport.Receive(ref chunks);
                fs.Write(chunks, 0, m);
                Console.WriteLine("Chunk contains: {0} bytes", m);
                fileSize -= m;
                Console.WriteLine("Filecontent remaning: {0} bytes", fileSize);
            }

            Console.WriteLine("File named {0} was created succesfully...", fileName);
        }
            
        public static void Main(string[] args)
        {         
            Console.WriteLine("Client running...");
            File_client fc = new File_client(args);
            Console.ReadKey();
        }
    }
}