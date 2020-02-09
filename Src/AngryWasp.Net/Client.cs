using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using AngryWasp.Helpers;
using AngryWasp.Logger;

namespace AngryWasp.Net
{
    public class Client
    {
        public void Connect(string host, ushort port)
        {
            Task.Run( () =>
            {
                try
                {
                    byte[] buff = new byte[64];
                    TcpClient client = new TcpClient();
                    client.Connect(host, port);

                    NetworkStream ns = client.GetStream();

                    ns.Write(Handshake.GenerateRequest(true).ToArray());

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

                    if (!accept)
                    {
                        Log.Instance.Write(Log_Severity.Error, $"Connection to host {client.Client.RemoteEndPoint} refused: {reason}");
                        client.Close();
                        return;
                    }

                    int offset = 0;
                    ulong peerId = BitShifter.ToULong(body, ref offset);
                    ushort serverPort = BitShifter.ToUShort(body, ref offset);

                    ConnectionManager.Add(new Connection(client, peerId, serverPort, Direction.Outgoing));
                }
                catch
                {
                    Log.Instance.Write(Log_Severity.Warning, $"Could not connect to node {host}:{port}");
                }
            });
        }

        public static void ConnectToSeedNodes()
        {
            foreach (var n in Config.SeedNodes)
            {
                Task.Run( () =>
                {
                    Log.Instance.Write($"Attempting connection to seed node: {n.ToString()}");
                    new Client().Connect(n.Host, n.Port);
                });
            }
        }

        public static void ConnectToNodeList(List<Node> nodes)
        {
            foreach (var n in nodes)
            {
                if (ConnectionManager.HasConnection(n.PeerID))
                    continue;

                if (n.PeerID == Server.PeerID)
                    continue;

                Task.Run( () =>
                {
                    Log.Instance.Write($"Attempting connection to node: {n.ToString()}");
                    new Client().Connect(n.Host, n.Port);
                });
            }
        }
    }
}