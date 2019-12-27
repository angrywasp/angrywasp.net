using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using AngryWasp.Helpers;

namespace AngryWasp.Net
{
    [Flags]
    public enum Direction
    {
        Invalid = 0,
        Incoming = 1,
        Outgoing = 2
    }

    public class Connection
    {
        private byte[] readBuffer;
        private TcpClient client;
        private DataProcessor dataProcessor;
        private int failureCount = 0;
        private Direction direction = Direction.Invalid;
        private ushort port = 0;
        private ulong peerId = 0;
        AsyncCallback readCallback;

        public byte[] ReadBuffer => readBuffer;

        public Direction Direction => direction;

        public ushort Port => port;

        public ulong PeerId => peerId;

        public IPAddress Address => ((IPEndPoint)client.Client.RemoteEndPoint).Address;

        public Connection(TcpClient client, ulong peerId, ushort port, Direction direction)
        {
            this.readBuffer = new byte[Config.READ_BUFFER_SIZE];
            this.client = client;
            this.peerId = peerId;
            this.port = port;
            this.direction = direction;
            this.dataProcessor = new DataProcessor();
            this.dataProcessor.Start(this);
        }

        public void AddFailure()
        {
            ++failureCount;
            if (failureCount >= Config.FAILURES_BEFORE_BAN)
                ConnectionManager.Remove(this);
        }

        public bool Write(byte[] input)
        {
            if (!client.Client.Connected)
                return false;

            try
            {
                client.GetStream().Write(input);
                return true;
            }
            catch { return false; }
        }

        public void BeginRead(Action<IAsyncResult> callback)
        {
            readCallback = new AsyncCallback(callback);
            client.GetStream().BeginRead(readBuffer, 0, Config.READ_BUFFER_SIZE, readCallback, null);
        }

        public void ResumeRead()
        {
            client.GetStream().BeginRead(readBuffer, 0, Config.READ_BUFFER_SIZE, readCallback, null);
        }

        public int EndRead(IAsyncResult result)
        {
            return client.GetStream().EndRead(result);
        }

        public void Close()
        {
            if (!client.Client.Connected)
                return;

            client.GetStream().Close();
        }

        public byte[] GetPeerListBytes()
        {
            IPEndPoint r = (IPEndPoint)client.Client.RemoteEndPoint;

            List<byte> bytes = new List<byte>();

            bytes.AddRange(r.Address.MapToIPv4().GetAddressBytes());
            bytes.AddRange(BitShifter.ToByte((ushort)port));
            bytes.AddRange(BitShifter.ToByte(peerId));

            return bytes.ToArray();
        }
    }
}
