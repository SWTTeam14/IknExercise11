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
                     
			receiveFile(args[0], trans);
        }


        private void receiveFile(String fileName, ITransport transport)
        {         
			byte[] chunks = new byte[BUFSIZE];
            
			chunks = Encoding.ASCII.GetBytes(fileName);

			transport.Send(chunks, chunks.Length);

			chunks = new byte[BUFSIZE];

			transport.Receive(ref chunks);
			long fileSize = BitConverter.ToInt64(chunks, 0);
            
            if(fileSize == 0)
			{
				throw new Exception("File not found...");
			}
			else
			{
                Console.WriteLine("File size from server: {0} ", fileSize);

				FileStream fs = File.Create(fileName);

                while(fileSize > 0)
				{                  
					var m = transport.Receive(ref chunks);

					fs.Write(chunks, 0, chunks.Length);
                    
                    fileSize -= m;

					Console.WriteLine("File size from server: {0} ", fileSize);
				}
				fs.Close();
                Console.WriteLine("Received file...\n");
				Console.WriteLine("File named {0} was created succesfully...", fileName);            
			}
        }


        public static void Main(string[] args)
        {         
			Console.WriteLine("Client running...");
			file_client fc = new file_client(args);
            Console.ReadKey();
        }
    }
}