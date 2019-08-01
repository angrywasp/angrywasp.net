using System;
using System.Collections.Generic;
using AngryWasp.Helpers;
using AngryWasp.Logger;

namespace AngryWasp.Net
{
    public class Header
    {
        public const int LENGTH = 18;

        private static readonly byte[] SIGNATURE = new byte[] { 0x44, 0x45, 0x52, 0x50 };
        private static readonly uint SIGNATURE_INT = BitShifter.ToUInt(SIGNATURE);

        private static readonly byte[] PROTOCOL_VERSION = new byte[] { 0x00, 0x01 };
        private static readonly ushort PROTOCOL_VERSION_INT = BitShifter.ToUShort(PROTOCOL_VERSION);

        private ulong peerId = 0;
        private byte command = 0;
        private ushort dataLength = 0;
        private bool isRequest = true;

        public ulong PeerID => peerId;

        public byte Command => command;

        public ushort DataLength => dataLength;

        public bool IsRequest => isRequest;

        public static byte[] Create(byte command, bool isRequest = true, ushort dataLength = 0)
        {
            byte[] bytes = new byte[LENGTH];
            Buffer.BlockCopy(SIGNATURE, 0, bytes, 0, 4);
            Buffer.BlockCopy(PROTOCOL_VERSION, 0, bytes, 4, 2);
            bytes[6] = Convert.ToByte(isRequest);
            Buffer.BlockCopy(BitShifter.ToByte(Server.PeerID), 0, bytes, 7, 8);
            bytes[15] = command;
            Buffer.BlockCopy(BitShifter.ToByte(dataLength), 0, bytes, 16, 2);
            return bytes;
        }

        public static Header Parse(byte[] bytes)
        {
            int offset = 0;
            uint sig = BitShifter.ToUInt(bytes, ref offset);

            if (sig != SIGNATURE_INT)
            {
                Log.Instance.Write(Log_Severity.Error, "Invalid protocol signature");
                return null;
            }

            ushort ver = BitShifter.ToUShort(bytes, ref offset);

            if (ver != PROTOCOL_VERSION_INT)
            {
                Log.Instance.Write(Log_Severity.Error, "Protocol version mismatch");
                return null;
            }

            return new Header
            {
                isRequest = Convert.ToBoolean(bytes[offset++]),
                peerId = BitShifter.ToULong(bytes, ref offset),
                command = bytes[offset++],
                dataLength = BitShifter.ToUShort(bytes, ref offset)
            };
        }
    }
}