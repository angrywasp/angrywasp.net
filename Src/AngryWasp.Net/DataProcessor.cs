using System;
using System.Net.Sockets;
using AngryWasp.Logger;

namespace AngryWasp.Net
{
    public class DataProcessor
    {
        Connection c;

        public void Start(Connection c)
        {
            this.c = c;
            c.BeginRead(DoRead);
        }

        private void DoRead(IAsyncResult result)
        {
            int r = 0;

            try
            {
                r = c.EndRead(result);

                if (r == 0)
                {
                    Log.Instance.Write($"Peer {c.PeerId} disconnected");
                    ConnectionManager.Remove(c);
                    return;
                }
                
                byte[] received = new byte[r];
                Buffer.BlockCopy(c.ReadBuffer, 0, received, 0, r);

                c.ResumeRead();
                
                Log.Instance.Write($"Got {received.Length} bytes of data");
                Header header;
                byte[] body;
                if (!ParseBuffer(received, out header, out body))
                {
                    Log.Instance.Write(Log_Severity.Error, $"Could not process data package");
                    c.AddFailure();
                    return;
                }

                CommandProcessor.Process(c, header, body);
            }
            catch
            {
                Log.Instance.Write($"Peer {c.PeerId} disconnected");
            }
        }

        public static bool ParseBuffer(byte[] data, out Header header, out byte[] body)
        {
            header = null;
            body = null;

            if (data.Length < Header.LENGTH)
            {
                Log.Instance.Write(Log_Severity.Error, "Incomplete data packet received");
                return false;
            }

            header = Header.Parse(data);

            //if header == null, then the error has already been logged, so just return
            if (header == null)
                return false;

            if (data.Length < Header.LENGTH + header.DataLength)
            {
                Log.Instance.Write(Log_Severity.Error, "Incomplete data packet received");
                return false;
            }

            body = new byte[header.DataLength];
            Buffer.BlockCopy(data, Header.LENGTH, body, 0, header.DataLength);

            return true;
        }
    }
}