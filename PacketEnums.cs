using System;

namespace TCPUDPSample
{
    /// <summary>Sent from server to client.</summary>
    public enum ServerPackets
    {
        welcome = 1,
        udpTest
    }

    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        welcomeReceived = 1,
        udpTestReceive
    }
}
