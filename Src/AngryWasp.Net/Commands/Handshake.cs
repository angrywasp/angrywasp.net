using System.Collections.Generic;
using AngryWasp.Helpers;

namespace AngryWasp.Net
{
    public class Handshake
    {
        public const byte CODE = 1;

        public static List<byte> GenerateRequest(bool isRequest)
        {
            return Header.Create(CODE, isRequest, 12)
                .Join(BitShifter.ToByte(Server.PeerID))
                .Join(BitShifter.ToByte(Server.Port));
        }
    }
}