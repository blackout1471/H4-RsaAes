using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class ServerSocket
    {
        // Delegates

        public delegate void StartListeningEvent();
        public delegate void AcceptEvent(SocketState state);
        public delegate void ReadMessageEvent(string message);

        // Props

        public int BufferSize { get; set; }
        public int Port { get; private set; }
        public IPAddress IpAddress { get; private set; }
        public IPEndPoint IpEndPoint { get; private set; }

        // Events

        public StartListeningEvent OnStartListening { get; set; }
        public AcceptEvent OnAccept { get; set; }
        public ReadMessageEvent OnReadMessage { get; set; }
        private static readonly ManualResetEvent mre = new ManualResetEvent(false);

        public ServerSocket(IPAddress address, int bufferSize, int port)
        {
            BufferSize = bufferSize;
            Port = port;
            IpAddress = address;
            IpEndPoint = new IPEndPoint(IpAddress, Port);
        }

        public void StartListening()
        {
            Socket listener = new Socket(IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(IpEndPoint);
                listener.Listen(100);

                while(true)
                {
                    mre.Reset();
                    OnStartListening?.Invoke();
                    listener.BeginAccept(new AsyncCallback(AcceptCall), listener);
                    mre.WaitOne();
                }
            }
            catch (Exception)
            {
                throw;
            } 
        }

        private void AcceptCall(IAsyncResult ar)
        {
            mre.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            SocketState state = new SocketState(BufferSize, handler);
            handler.BeginReceive(state.Buffer, 0, BufferSize, 0, new AsyncCallback(ReadCall), state);
            OnAccept?.Invoke(state);
        }

        private void ReadCall(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;

            int bytesRead = state.WorkerSocket.EndReceive(ar);

            if (bytesRead > 0)
            {
                string content = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);

                if (content.Contains("<EOF>"))
                {
                    OnReadMessage?.Invoke(content);
                }
                else
                {
                    state.WorkerSocket.BeginReceive(state.Buffer, 0, BufferSize, 0, new AsyncCallback(ReadCall), state);
                }

            }
        }
    }
}
