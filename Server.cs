using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace TCPUDPSample
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
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0)
                    {
                        return;
                    }

                    if (Clients[_clientId].udp.endPoint == null)
                    {
                        Clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (Clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
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
                if (_clientEndPoint != null)
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

        public class ClientHandler
        {
            public int id;
            public TCP tcp;
            public UDP udp;

            public ClientHandler(int _clientId)
            {
                id = _clientId;
                tcp = new TCP(id);
                udp = new UDP(id);
            }

            public class TCP
            {
                public TcpClient socket;
                private readonly int id;
                private NetworkStream stream;
                private byte[] receiveBuffer;
                private Packet receivedData;

                public TCP(int _id)
                {
                    id = _id;
                }

                public void Connect(TcpClient _socket)
                {
                    socket = _socket;
                    socket.ReceiveBufferSize = 4096;
                    socket.SendBufferSize = 4096;

                    stream = socket.GetStream();
                    receivedData = new Packet();
                    receiveBuffer = new byte[socket.ReceiveBufferSize];

                    stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);

                    Console.WriteLine($"Client {id} connected via TCP.");
                }

                public void SendData(Packet _packet)
                {
                    try
                    {
                        if (socket != null)
                        {
                            stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                        }
                    }
                    catch (Exception _ex)
                    {
                        Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
                    }
                }

                private void ReceiveCallback(IAsyncResult _result)
                {
                    try
                    {
                        int _byteLength = stream.EndRead(_result);
                        if (_byteLength <= 0)
                        {
                            Server.Clients[id].Disconnect();
                            return;
                        }

                        byte[] _data = new byte[_byteLength];
                        Array.Copy(receiveBuffer, _data, _byteLength);

                        receivedData.Reset(HandleData(_data));
                        if (stream != null)
                        {
                            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
                        }
                    }
                    catch (Exception _ex)
                    {
                        Console.WriteLine($"Error receiving TCP data: {_ex}");
                        Server.Clients[id].Disconnect();
                    }
                }

                private bool HandleData(byte[] _data)
                {
                    int _packetLength = 0;

                    receivedData.SetBytes(_data);

                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }

                    while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                    {
                        byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                        ThreadManager.ExecuteOnMainThread(() =>
                        {
                            using (Packet _packet = new Packet(_packetBytes))
                            {
                                int _packetId = _packet.ReadInt();
                                Server.packetHandlers[_packetId](id, _packet);
                            }
                        });

                        _packetLength = 0;
                        if (receivedData.UnreadLength() >= 4)
                        {
                            _packetLength = receivedData.ReadInt();
                            if (_packetLength <= 0)
                            {
                                return true;
                            }
                        }
                    }

                    if (_packetLength <= 1)
                    {
                        return true;
                    }

                    return false;
                }

                public void Disconnect()
                {
                    socket.Close();
                    stream = null;
                    receivedData = null;
                    receiveBuffer = null;
                    socket = null;
                }
            }

            public class UDP
            {
                public IPEndPoint endPoint;
                private readonly int id;

                public UDP(int _id)
                {
                    id = _id;
                }

                public void Connect(IPEndPoint _endPoint)
                {
                    endPoint = _endPoint;
                    Console.WriteLine($"Client {id} connected via UDP.");
                }

                public void SendData(Packet _packet)
                {
                    Server.SendUDPData(endPoint, _packet);
                }

                public void HandleData(Packet _packetData)
                {
                    int _packetLength = _packetData.ReadInt();
                    byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id, _packet);
                        }
                    });
                }

                public void Disconnect()
                {
                    endPoint = null;
                }
            }

            public void Disconnect()
            {
                Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
                tcp.Disconnect();
                udp.Disconnect();
            }
        }
    }
}
