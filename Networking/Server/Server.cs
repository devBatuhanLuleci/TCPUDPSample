using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TCPUDPSample.Core;
using TCPUDPSample.Protocols;
using TCPUDPSample.Protocols.Server;

namespace TCPUDPSample.Networking.Server
{
    public class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, ClientHandler> Clients = new Dictionary<int, ClientHandler>();

        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler>? packetHandlers;

        private static TcpListener? tcpListener;
        private static UdpClient? udpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            if (tcpListener == null) return;

            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (Clients[i].tcp.socket == null)
                {
                    Clients[i].tcp.Connect(_client);
                    ServerSend.Welcome(i, "Welcome to the server!");
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            if (udpListener == null) return;

            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint!);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data == null || _data.Length < 4) return;

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0) return;

                    if (Clients[_clientId].udp.endPoint == null)
                    {
                        Clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (Clients[_clientId].udp.endPoint != null && Clients[_clientId].udp.endPoint!.ToString() == _clientEndPoint.ToString())
                    {
                        Clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null && udpListener != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }

        private static void InitializeServerData()
        {
            Clients.Clear();
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new ClientHandler(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.udpTestReceive, ServerHandle.UDPTestReceive }
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}
