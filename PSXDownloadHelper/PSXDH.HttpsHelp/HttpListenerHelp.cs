using System;
using System.Net;
using System.Net.Sockets;
using PSXDH.ProxyHelp;

namespace PSXDH.HttpsHelp
{
    public sealed class HttpListenerHelp : Listener
    {
        public UpdataUrlLog UpdataUrlLog;

        public HttpListenerHelp(int port)
            : this(IPAddress.Any, port)
        {
        }

        public HttpListenerHelp(IPAddress address, int port)
            : base(port, address)
        {
        }

        public HttpListenerHelp(IPAddress address, int port, UpdataUrlLog updataurlLog)
            : base(port, address)
        {
            UpdataUrlLog = updataurlLog;
        }

        public override string ConstructString
        {
            get { return ("Host:" + Address + ";Port:" + Port); }
        }

        public override void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = ListenSocket.EndAccept(ar);
                if (clientSocket != null)
                {
                    var client = new HttpClient(clientSocket, RemoveClient, UpdataUrlLog);
                    AddClient(client);
                    client.StartHandshake();
                }
            }
            catch
            {
            }
            try
            {
                ListenSocket.BeginAccept(OnAccept, ListenSocket);
            }
            catch
            {
                Dispose();
            }
        }

        public override string ToString()
        {
            return ("HTTP service on " + Address + ":" + Port);
        }
    }
}