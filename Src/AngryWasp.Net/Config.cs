namespace AngryWasp.Net
{
    public class Node
    {
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = Config.DEFAULT_PORT;

        public ulong PeerID { get; set; } = 0;

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }

    public class Config
    {
        public const int DEFAULT_PORT = 3500;
        public const int READ_BUFFER_SIZE = ushort.MaxValue;
        
        //The number of data failures to accept before diching the connection
        public const int FAILURES_BEFORE_BAN = 3;

        public static readonly Node[] SeedNodes = new Node[]
        {
            new Node { Host = "127.0.0.1", PeerID = 0, Port = 3500 }
        };
    }
}