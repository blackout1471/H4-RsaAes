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
                OnStartListening?.Invoke();

                while (true)
                {
                    Socket handler = listener.Accept();
                    SocketState clientSocket = new SocketState(BufferSize, handler);
                    OnAccept?.Invoke(clientSocket);

                    while (clientSocket.WorkerSocket.Connected)
                    {
                        int bytes = clientSocket.WorkerSocket.Receive(clientSocket.Buffer);

                        if (bytes > 0)
                        {
                           var plainText = Encoding.UTF8.GetString(clientSocket.Buffer, 0, bytes);
                            clientSocket.Builder.Append(plainText);

                            if (plainText.Contains("<EOF>"))
                            {
                                string message = clientSocket.Builder.ToString();
                                message = message.Replace("<EOF>", "");
                                OnReadMessage?.Invoke(message);
                                clientSocket.Builder.Clear();
                                Array.Clear(clientSocket.Buffer, 0, BufferSize);
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
