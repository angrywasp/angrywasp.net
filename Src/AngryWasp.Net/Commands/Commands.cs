using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngryWasp.Logger;

namespace AngryWasp.Net
{
    public static class CommandProcessor
    {
        private static Dictionary<byte, Action<Connection, Header, byte[]>> commands = new Dictionary<byte, Action<Connection, Header, byte[]>>();

        public static void RegisterDefaultCommands()
        {
            RegisterCommand(Ping.CODE, Ping.GenerateResponse);
            RegisterCommand(ExchangePeerList.CODE, ExchangePeerList.GenerateResponse);
        }

        public static void RegisterCommand(byte cmd, Action<Connection, Header, byte[]> handler)
        {
            if (commands.ContainsKey(cmd))
            {
                Log.Instance.Write(Log_Severity.Warning, $"Attempt to add duplicate command handler for byte code {cmd}");
                return;
            }

            Log.Instance.Write($"Added command handler for byte code {cmd}");
            commands.Add(cmd, handler);
        }

        public static void Process(Connection c, Header h, byte[] d)
        {
            Log.Instance.Write($"Processing response for command {h.Command.ToString()}");

            Task.Run( () =>
            {
                commands[h.Command].Invoke(c, h, d);
            });
        }
    }
}