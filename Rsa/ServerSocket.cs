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

        #region Socket handler data
        public int BufferSize { get; private set; }
        public byte[] Buffer { get; private set; }
        public ServerSocket WorkerSocket { get; private set; }
        public StringBuilder Builder { get; private set; }
        #endregion

        public int Port { get; private set; }
        public IPAddress IpAddress { get; private set; }
        public IPEndPoint IpEndPoint { get; set; }

        private static readonly ManualResetEvent mre = new ManualResetEvent(false);

        public ServerSocket(int bufferSize, int port)
        {
            BufferSize = bufferSize;
            Buffer = new byte[BufferSize];
            Builder = new StringBuilder();
            Port = port;

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IpAddress = ipHostInfo.AddressList.FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork);
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

                    Console.WriteLine("Waiting for connection");

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
            handler.BeginReceive(Buffer, 0, BufferSize, 0, new AsyncCallback(ReadCall), handler);
            Console.WriteLine("call accepted");
        }

        private void ReadCall(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                string content = Encoding.ASCII.GetString(Buffer, 0, bytesRead);

                if (content.Contains("<EOF>"))
                {
                    // All data read
                    Console.WriteLine(content);
                }
                else
                {
                    handler.BeginReceive(Buffer, 0, BufferSize, 0, new AsyncCallback(ReadCall), handler);
                }

            }
        }
    }
}
