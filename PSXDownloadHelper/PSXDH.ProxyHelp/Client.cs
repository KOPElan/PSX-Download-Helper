using System;
using System.Net;
using System.Net.Sockets;

namespace PSXDH.ProxyHelp
{
    public abstract class Client : IDisposable
    {
        private readonly DestroyDelegate _destroyer;
        private readonly byte[] _mBuffer;
        private readonly byte[] _mRemoteBuffer;
        private Socket _mClientSocket;
        private Socket _mDestinationSocket;

        protected Client()
        {
            _mBuffer = new byte[0x1000];
            _mRemoteBuffer = new byte[0x400];
            ClientSocket = null;
            _destroyer = null;
        }

        protected Client(Socket clientSocket, DestroyDelegate destroyer)
        {
            _mBuffer = new byte[0x1000];
            _mRemoteBuffer = new byte[0x400];
            ClientSocket = clientSocket;
            _destroyer = destroyer;
        }

        public byte[] Buffer
        {
            get { return _mBuffer; }
        }

        public Socket ClientSocket
        {
            get { return _mClientSocket; }
            set
            {
                if (_mClientSocket != null)
                {
                    _mClientSocket.Close();
                }
                _mClientSocket = value;
            }
        }

        public Socket DestinationSocket
        {
            get { return _mDestinationSocket; }
            set
            {
                if (_mDestinationSocket != null)
                {
                    _mDestinationSocket.Close();
                }
                _mDestinationSocket = value;
            }
        }

        public byte[] RemoteBuffer
        {
            get { return _mRemoteBuffer; }
        }

        public void Dispose()
        {
            try
            {
                if (ClientSocket != null)
                    ClientSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            try
            {
                if (DestinationSocket != null)
                    DestinationSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            if (ClientSocket != null)
            {
                ClientSocket.Close();
            }
            if (DestinationSocket != null)
            {
                DestinationSocket.Close();
            }
            ClientSocket = null;
            DestinationSocket = null;
            if (_destroyer != null)
            {
                _destroyer(this);
            }
        }

        public void OnClientReceive(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket == null)
                    return;

                int size = ClientSocket.EndReceive(ar);
                if (size > 0 && DestinationSocket!=null)
                {
                    DestinationSocket.BeginSend(Buffer, 0, size, SocketFlags.None, OnRemoteSent, DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        public void OnClientSent(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket!=null&&ClientSocket.EndSend(ar) > 0)
                {
                    DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None,
                                                   OnRemoteReceive, DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        public void OnRemoteReceive(IAsyncResult ar)
        {
            try
            {
                if (DestinationSocket == null)
                    return;

                int size = DestinationSocket.EndReceive(ar);
                if (size > 0 && ClientSocket!=null)
                {
                    ClientSocket.BeginSend(RemoteBuffer, 0, size, SocketFlags.None, OnClientSent, ClientSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        public void OnRemoteSent(IAsyncResult ar)
        {
            try
            {
                if (DestinationSocket.EndSend(ar) > 0 && ClientSocket!=null)
                {
                    ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnClientReceive, ClientSocket);
                }
            }
            catch { Dispose(); }
            //finally
            //{
            //    Dispose();
            //}
        }

        public abstract void StartHandshake();

        public void StartRelay()
        {
            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnClientReceive, ClientSocket);
                    DestinationSocket.BeginReceive(RemoteBuffer, 0, RemoteBuffer.Length, SocketFlags.None, OnRemoteReceive,
                                                   DestinationSocket);
                }
            }
            catch
            {
                Dispose();
            }
        }

        public override string ToString()
        {
            try
            {
                return ("正在建立连接： " + ((IPEndPoint)DestinationSocket.RemoteEndPoint).Address);
            }
            catch
            {
                return "连接建立成功";
            }
        }
    }
}