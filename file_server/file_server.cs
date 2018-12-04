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
			while (true)
			{            
				try{
					ITransport trans = new Transport(BUFSIZE, APP);
					byte[] buffer = new byte[BUFSIZE];
					int fileLength = 0;

					fileLength = trans.Receive(ref buffer);               
					string fileName = Encoding.Default.GetString(buffer, 0, fileLength);
     
					buffer = new byte[BUFSIZE];

                    //check_File_Exists returns filesize
					long fileSize = LIB.check_File_Exists(fileName);
					if (fileSize == 0)
                    {
                        throw new FileNotFoundException();
                    }
					Console.WriteLine($"Received request for: {fileName}");
					//Convert filesize to byte array
					buffer = BitConverter.GetBytes(fileSize);               
					trans.Send(buffer, buffer.Length);
                    sendFile(fileName, fileSize, trans);
				}
				catch(FileNotFoundException e)
				{
					Console.WriteLine("File not found...");
					throw e;
				}            
			}
        }

        private void sendFile(String fileName, long fileSize, ITransport transport)
        {
			FileStream fs = new FileStream(fileName, FileMode.Open);
            byte[] chunks = new byte[BUFSIZE];
                     
            Console.WriteLine("Sending file: {0}", fileName);

			int bytesReceived = 0;
			int sizeOfChunk = BUFSIZE;

            while(fileSize > bytesReceived)
            {
                fs.Read(chunks, 0, sizeOfChunk);
                
				transport.Send(chunks, sizeOfChunk);

				bytesReceived += BUFSIZE;
                            
				if((bytesReceived < fileSize) && (bytesReceived + BUFSIZE > fileSize))
				{
					sizeOfChunk = (int)fileSize - bytesReceived;
				}
            }
            fs.Close();
            Console.WriteLine("File was sent succesfully...");
        }


        public static void Main(string[] args)
        {
			Console.WriteLine("Server running...");
            file_server fs = new file_server();
			Console.ReadKey();
        }
    }
}