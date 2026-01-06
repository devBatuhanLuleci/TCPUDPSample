using System;
using TCPUDPSample.Core;
using TCPUDPSample.Networking.Server;

namespace TCPUDPSample.Protocols.Server
{
    public class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Networking.Server.Server.Clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Networking.Server.Server.Clients[_toClient].udp.SendData(_packet);
        }

        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void UDPTest(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.udpTest))
            {
                _packet.Write("A test packet for UDP");

            }
        }
        public static void DataExchange(int _toClient, string _data)
        {
            using (Packet _packet = new Packet((int)ServerPackets.dataExchange))
            {
                _packet.Write(_data);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendDataToAll(string _data)
        {
            foreach (var _client in Networking.Server.Server.Clients.Values)
            {
                if (_client.tcp.socket != null)
                {
                    DataExchange(_client.id, _data);
                }
            }
        }
    }
}
