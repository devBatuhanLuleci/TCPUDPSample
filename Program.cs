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

            if (input == "1")
            {
                isRunning = true;
                Server.Start(10, 26950);
            }
            else if (input == "2")
            {
                isRunning = true;
                Client client = new Client();
                client.Start();
                client.ConnectToServer();
            }
            else
            {
                Console.WriteLine("Invalid input. Exiting.");
                return;
            }

            // Main loop
            while (isRunning)
            {
                ThreadManager.UpdateMain();
                Thread.Sleep(10); // Small sleep to prevent 100% CPU usage
            }
        }
    }
}