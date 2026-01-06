using System;

namespace TCPUDPSample
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.Clients[_fromClient].tcp.socket.Client.RemoteEndPoint} back with username: {_username} (ID: {_fromClient})");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            // Send player into game logic here
        }

        public static void UDPTestReceive(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();
            Console.WriteLine($"Received UDP packet from client {_fromClient}: {_msg}");
        }
    }
}
