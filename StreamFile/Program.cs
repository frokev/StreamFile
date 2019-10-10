using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;      //required
using System.Net.Sockets;    //required
using System.IO;
using System.Threading;

namespace StreamFile
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                Console.WriteLine("Which port should the server run on? Press enter for 9999:");
                string port = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(port))
                    port = "9999";

                Console.WriteLine("Enter path to logfile:");
                string path = Console.ReadLine();

                TcpListener server = new TcpListener(IPAddress.Any, int.Parse(port));
                // we set our IP address as server's address, and we also set the port: 9999 by default

                server.Start();  // this will start the server

                while (true)   //we wait for a connection
                {
                    if(!server.Pending())
                    {
                        Thread.Sleep(1000);
                        continue; // Continue to check for pending connections
                    }

                    TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it
                    NetworkStream ns = client.GetStream();
                    StreamWriter sw = new StreamWriter(client.GetStream());
                    StreamReader sr = new StreamReader(path);

                    bool hasConnectedMsg = false;

                    using(sw)
                    {
                        while (client.Connected)  //while the client is connected, we write out the file, line by line
                        {
                            if (!hasConnectedMsg)
                            {
                                Console.WriteLine("Connected");
                                hasConnectedMsg = true;
                            }

                            if (ns.CanWrite)
                            {
                                string line = sr.ReadLine();
                                sw.Write(line);
                                sw.Flush();
                            }
                        }
                    }

                    Console.WriteLine("Connection lost");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
