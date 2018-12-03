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

			receiveFile("Test.txt", new Transport(BUFSIZE, APP));
        }


        private void receiveFile(String fileName, ITransport transport)
        {         
			byte[] buffer = new byte[BUFSIZE];
            
			buffer = Encoding.ASCII.GetBytes(fileName);

			transport.Send(buffer, buffer.Length);

			buffer = new byte[BUFSIZE];

			transport.Receive(ref buffer);
			long fileSize = BitConverter.ToInt64(buffer, 0);
            
            if(fileSize == 0)
			{
				throw new Exception("File not found...");
			}
			else
			{
                Console.WriteLine("File size from server: {0} ", fileSize);

				FileStream fs = File.Create(fileName);

				int counter = 1000, lastRead = 1;

                while(lastRead >= 0)
				{                  
					transport.Receive(ref buffer);

					fs.Write(buffer, 0, counter);
                    
                    fileSize -= 1000;

                    if(fileSize < 1000)
					{
						counter = (int)fileSize;
						--lastRead;
					}

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