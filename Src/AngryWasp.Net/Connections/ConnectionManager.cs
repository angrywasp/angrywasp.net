using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AngryWasp.Logger;

namespace AngryWasp.Net
{
    public static class ConnectionManager
    {
        private static ConcurrentDictionary<ulong, Connection> connections = new ConcurrentDictionary<ulong, Connection>();

        public static bool Add(Connection value)
        {
            if (connections.ContainsKey(value.PeerId))
                return false;

            connections.TryAdd(value.PeerId, value);

            Log.Instance.Write($"Connection added - {value.PeerId}");
            return true;
        }

        public static bool Remove(Connection c)
        {
            c.Close();

            if (!connections.ContainsKey(c.PeerId))
                return false;

            Connection ta;    
            connections.TryRemove(c.PeerId, out ta);
            Log.Instance.Write($"Connection removed - {c.PeerId}");
            return true;
        }

        public static bool HasConnection(ulong peerId) => connections.ContainsKey(peerId);

        public static int Count => connections.Count;

        public static void ForEach(Direction direction, Action<Connection> action)
        {
            foreach(var c in connections)
                if (direction.HasFlag(c.Value.Direction))
                    action(c.Value);
        }

        public static byte[] GetPeerList()
        {
            List<byte> bytes = new List<byte>();
            
            foreach(var c in connections)
                bytes.AddRange(c.Value.GetPeerListBytes());

            return bytes.ToArray();
        }
    }
}