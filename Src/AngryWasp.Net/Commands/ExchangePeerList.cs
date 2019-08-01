using System;
using System.Collections.Generic;
using System.Diagnostics;
using AngryWasp.Helpers;
using AngryWasp.Logger;

namespace AngryWasp.Net
{
    public class ExchangePeerList
    {
        public const byte CODE = 2;

        public static byte[] GenerateRequest(bool isRequest) 
        {
            byte[] peers = ConnectionManager.GetPeerList();
            byte[] header = Header.Create(ExchangePeerList.CODE, isRequest, (ushort)peers.Length);

            List<byte> bytes = new List<byte>();
            bytes.AddRange(header);
            bytes.AddRange(peers);

            return bytes.ToArray();
        }

        public static void GenerateResponse(Connection c, Header h, byte[] d)
        {
            //reconstruct the node list sent to us by the client
            int offset = 0;
            int count = 0;
            List<Node> nodes = new List<Node>();
                
            count = d.Length / 14;
            for (int i = 0; i < count; i++)
            {
                nodes.Add(new Node
                {
                    Host = $"{d[offset++]}.{d[offset++]}.{d[offset++]}.{d[offset++]}",
                    Port = BitShifter.ToUShort(d, ref offset),
                    PeerID = BitShifter.ToULong(d, ref offset)
                });
            }

            Client.ConnectToNodeList(nodes);

            // if isRequest == true, we are on the server so we reply with a peer list of our own
            if (h.IsRequest)
                c.Write(GenerateRequest(false));
        }
    }
}