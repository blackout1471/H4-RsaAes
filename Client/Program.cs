using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketConnection socketConnection = new SocketConnection("192.168.1.3", 5000);
            socketConnection.Connect();

            socketConnection.SendMessage("Hello world");

            socketConnection.DisConnenect();


            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
