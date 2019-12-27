using System;

namespace AngryWasp.Net
{
    public static class CommandCode
    {
        public static Func<byte, string> externalHandler;
        
        public static string CommandString(byte code)
        {
            switch (code)
            {
                case ExchangePeerList.CODE: return "ExchangePeerList";
                case Handshake.CODE: return "Handshake";
                case Ping.CODE: return "Ping";
            }

            if (externalHandler != null)
                return externalHandler(code);
            else
                return "Unknown";
        }

        public static void AddExternalHandler(Func<byte, string> handler) => externalHandler = handler;
    }
}