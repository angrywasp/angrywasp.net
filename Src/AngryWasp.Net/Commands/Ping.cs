using System.Collections.Generic;

namespace AngryWasp.Net
{
    public class Ping
    {
        public const byte CODE = 3;

        public static List<byte> GenerateRequest() => Header.Create(CODE);

        public static void GenerateResponse(Connection c, Header h, byte[] d)
        {
            c.Write(Header.Create(CODE, false).ToArray());
        }
    }
}