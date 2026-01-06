using System;
using System.Threading;
using TCPUDPSample.Core;
using TCPUDPSample.Networking.Server;
using TCPUDPSample.Networking.Client;

namespace TCPUDPSample
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.WriteLine("TCP/UDP Sample Project started.");
            Console.WriteLine("Press 1 to start Server, or 2 to start Client:");

            string? input = Console.ReadLine();

            bool isServer = false;
            if (input == "1")
            {
                isRunning = true;
                isServer = true;
                Server.Start(10, 26950);
            }
            else if (input == "2")
            {
                isRunning = true;
                isServer = false;
                Client client = new Client();
                client.Start();
                client.ConnectToServer();
            }
            else
            {
                Console.WriteLine("Invalid input. Exiting.");
                return;
            }

            Console.WriteLine(isServer ? "Server is running. Press 'M' to send a message to all clients." : "Client is running. Press 'G' to send a message to the server.");

            // Main loop
            while (isRunning)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo _key = Console.ReadKey(true);
                    if (isServer && _key.Key == ConsoleKey.M)
                    {
                        Protocols.Server.ServerSend.SendDataToAll("Hello from the server!");
                    }
                    else if (!isServer && _key.Key == ConsoleKey.G)
                    {
                        Protocols.Client.ClientSend.DataExchange("Hello from the client!");
                    }
                }

                ThreadManager.UpdateMain();
                Thread.Sleep(10); // Small sleep to prevent 100% CPU usage
            }
        }
    }
}