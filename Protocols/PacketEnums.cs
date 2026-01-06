using System;

namespace TCPUDPSample.Protocols
{
    /// <summary>Sent from server to client.</summary>
    public enum ServerPackets
    {
        welcome = 1,
        udpTest,
        dataExchange
    }

    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        welcomeReceived = 1,
        udpTestReceive,
        dataExchange
    }
}
