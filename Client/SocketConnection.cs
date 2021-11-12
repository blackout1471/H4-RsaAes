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

        public void SendMessage(string _message)
        {
            byte[] messageSent = Encoding.UTF8.GetBytes(_message);
            int bytesSend = socket.Send(messageSent);
        }

        public void DisConnenect()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}
