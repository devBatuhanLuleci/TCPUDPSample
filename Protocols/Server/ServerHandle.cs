using System;
using TCPUDPSample.Core;
using TCPUDPSample.Protocols;
using TCPUDPSample.Networking.Server;

namespace TCPUDPSample.Protocols.Server
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Networking.Server.Server.Clients[_fromClient].tcp.socket?.Client.RemoteEndPoint} back with username: {_username} (ID: {_fromClient})");

            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
        }

        public static void UDPTestReceive(int _fromClient, Packet _packet)
        {
            string _data = _packet.ReadString();
            Console.WriteLine($"Received UDP packet from client {_fromClient}: {_data}");
        }

        public static void DataExchange(int _fromClient, Packet _packet)
        {
            string _data = _packet.ReadString();
            Console.WriteLine($"Received data from client {_fromClient}: {_data}");

            // Optionally echo back or process it
            ServerSend.DataExchange(_fromClient, $"Server received your data: {_data}");
        }
    }
}
