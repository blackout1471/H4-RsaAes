using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerSocket socket = new ServerSocket(1024, 8000);
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
