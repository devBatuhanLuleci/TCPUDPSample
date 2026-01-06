using System;
using System.Net;

namespace TCPUDPSample
{
    public class ClientHandle
    {
        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _myId = _packet.ReadInt();

            Console.WriteLine($"Message from server: {_msg}");
            Client.instance.myId = _myId;
            ClientSend.WelcomeReceived();

            // Establish UDP connection
            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        public static void UDPTest(Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine($"Received packet via UDP: {_msg}");
            ClientSend.UDPTestReceive();
        }
    }
}
