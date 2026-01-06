using System;
using System.Net;
using TCPUDPSample.Core;
using TCPUDPSample.Protocols;

namespace TCPUDPSample.Protocols.Client
{
    public class ClientHandle
    {
        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _myId = _packet.ReadInt();

            Console.WriteLine($"Message from server: {_msg}");
            Networking.Client.Client.instance!.myId = _myId;
            ClientSend.WelcomeReceived();

            Networking.Client.Client.instance.udp!.Connect(((IPEndPoint)Networking.Client.Client.instance.tcp!.socket!.Client.LocalEndPoint!).Port);
        }

        public static void UDPTest(Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine($"Received packet via UDP: {_msg}");
            ClientSend.UDPTestReceive();
        }

        public static void DataExchange(Packet _packet)
        {
            string _data = _packet.ReadString();
            Console.WriteLine($"Received data from server: {_data}");
        }
    }
}
