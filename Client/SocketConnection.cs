using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class SocketConnection
    {
        IPEndPoint endPoint;
        Socket socket;

        public SocketConnection(string _ipAdress, int _port)
        {
            IPAddress ipAddr = IPAddress.Parse(_ipAdress);
            endPoint = new IPEndPoint(ipAddr, _port);
            socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            socket.Connect(endPoint);
        }

        public byte[] SendMessage(string _message)
        {
            byte[] messageSent = Encoding.ASCII.GetBytes(_message);
            int byteSent = socket.Send(messageSent);

            byte[] messageReceived = new byte[1024];

            int byteRecv = socket.Receive(messageReceived);
            Console.WriteLine("Message from Server -> {0}",
                  Encoding.ASCII.GetString(messageReceived,
                                             0, byteRecv));

            return messageReceived;
        }

        public void DisConnenect()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
