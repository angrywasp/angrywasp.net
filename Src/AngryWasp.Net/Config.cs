using System.Collections.Generic;

namespace AngryWasp.Net
{
    public class Node
    {
        public string Host { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = Config.DEFAULT_PORT;

        public ulong PeerID { get; set; } = 0;

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }

    public class Config
    {
        public const ushort DEFAULT_PORT = 3500;
        public const int READ_BUFFER_SIZE = ushort.MaxValue;
        
        //The number of data failures to accept before ditching the connection
        public const int FAILURES_BEFORE_BAN = 3;

        public static readonly List<Node> SeedNodes = new List<Node>();

        public static void AddSeedNode(string host, ushort port) =>
            SeedNodes.Add(new Node { Host = host, PeerID = 0, Port = port });
    }
}