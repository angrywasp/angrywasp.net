using System.Collections.Generic;
using AngryWasp.Helpers;

namespace AngryWasp.Net
{
    public class Handshake
    {
        public const byte CODE = 1;

        public static byte[] GenerateRequest(bool isRequest)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Header.Create(CODE, isRequest, 12));
            bytes.AddRange(BitShifter.ToByte(Server.PeerID));
            bytes.AddRange(BitShifter.ToByte(Server.Port));

            return bytes.ToArray();
        }
    }
}