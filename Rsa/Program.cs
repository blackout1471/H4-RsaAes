using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerSocket socket = new ServerSocket(1024, 5500);
            Console.WriteLine(socket.IpAddress);
            socket.StartListening();
        }
    }
}
