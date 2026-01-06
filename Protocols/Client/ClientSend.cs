using System;
using TCPUDPSample.Core;
using TCPUDPSample.Protocols;

namespace TCPUDPSample.Protocols.Client
{
    public class ClientSend
    {
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Networking.Client.Client.instance!.tcp!.SendData(_packet);
        }

        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            Networking.Client.Client.instance!.udp!.SendData(_packet);
        }

        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                _packet.Write(Networking.Client.Client.instance!.myId);
                _packet.Write("TestUser");

                SendTCPData(_packet);
            }
        }

        public static void UDPTestReceive()
        {
            using (Packet _packet = new Packet((int)ClientPackets.udpTestReceive))
            {
                _packet.Write("Received a UDP packet!");

            }
        }
        public static void DataExchange(string _data)
        {
            using (Packet _packet = new Packet((int)ClientPackets.dataExchange))
            {
                _packet.Write(_data);

                SendTCPData(_packet);
            }
        }
    }
}
