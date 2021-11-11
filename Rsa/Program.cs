using System;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
                Console.WriteLine($"{i}. {ipHostInfo.AddressList[i].ToString()}");

            Console.Write("Choose ip: ");
            string ipChoice = Console.ReadLine();
            IPAddress address = ipHostInfo.AddressList[Int32.Parse(ipChoice)];

            int port = 8000;

            ServerSocket socket = new ServerSocket(address, 1024, port);
            Console.WriteLine($"Created server with info: {address.ToString()}:{port}");
            socket.OnStartListening += OnStartListening;
            socket.OnAccept += OnAccept;
            socket.OnReadMessage += OnReadMessage;
            socket.StartListening();

            Console.ReadKey();
        }

        private static void OnStartListening()
        {
            Console.WriteLine("Waiting for connections");
        }

        private static void OnAccept(SocketState state)
        {
            Console.WriteLine("Connection has been made");
        }

        private static void OnReadMessage(string message)
        {
            Console.WriteLine("Client: " + message);
        }
    }
}
