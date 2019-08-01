using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using AngryWasp.Helpers;
using AngryWasp.Logger;

namespace AngryWasp.Net
{
    public class Server
    {
        TcpListener listener;

        private static int port = 0;
        private static ulong peerId = 0;

        public static int Port => port;

        public static ulong PeerID => peerId;

        public void Start(int serverPort = Config.DEFAULT_PORT, ulong pid = 0)
        {
            port = serverPort;
            peerId = pid;

            if (peerId == 0)
                peerId = Helper.GenerateRandomID();

            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Log.Instance.Write("Local P2P endpoint: " + listener.LocalEndpoint);
            Log.Instance.Write("P2P Server initialized");

            Task.Run(() =>
            {
                //todo: need to exit here on clean program shutdown
                while(true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Log.Instance.Write($"Receiving connection from: {client.Client.RemoteEndPoint}");
                    HandshakeClient(client);
                }
            });
        }

        public void HandshakeClient(TcpClient client)
        {
            Task.Run( () =>
            {
                Log.Instance.Write($"Verifying handshake");
                byte[] buff = new byte[64];
                NetworkStream ns = client.GetStream();

                bool accept = true;
                Header header = null;
                byte[] body = null;
                string reason = null;

                if (ns.Read(buff) < Header.LENGTH)
                {
                    accept = false;
                    reason = "Corrupt package header";
                }
                else if (!DataProcessor.ParseBuffer(buff, out header, out body))
                {
                    accept = false;
                    reason = "Buffer parsing failed";
                }
                else if (header.Command != Handshake.CODE)
                {
                    accept = false;
                    reason = "Wrong command code";
                }
                else if (ConnectionManager.HasConnection(header.PeerID))
                {
                    accept = false;
                    reason = "Connection already exists";
                }

                if (!accept)
                {
                    Log.Instance.Write(Log_Severity.Error, $"Connection to host {client.Client.RemoteEndPoint} refused: {reason}");
                    client.Close();
                    return;
                }

                int offset = 0;
                ulong peerId = BitShifter.ToULong(body, ref offset);
                int peerPort = BitShifter.ToInt(body, ref offset);

                //send a handshake packet back to the client to acknowledge the connection
                ns.Write(Handshake.GenerateRequest(false));
                ConnectionManager.Add(new Connection(client, peerId, peerPort, Direction.Incoming));
            });
        }
    }
}