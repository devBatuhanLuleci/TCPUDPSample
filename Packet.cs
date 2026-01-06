using System;

namespace TCPUDPSample
{
    public class Packet
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"[Packet {Id}] Type: {Type}, Payload: {Payload}, Time: {Timestamp}";
        }
    }
}
