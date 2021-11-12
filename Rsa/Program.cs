using CryptoService;
using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Server
{
    class Program
    {
        private static AesEncryption aesEncrypter = new AesEncryption();
        private static AesSecretDTO aes;

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
            message = message.Replace("<EOF>", "");

            if (message.Contains("<KEY>"))
            {
                message = message.Replace("<KEY>", "");

                byte[] rsaEncrypted = Convert.FromBase64String(message);
                RsaEncryption rsa = new RsaEncryption();
                byte[] byteAes = rsa.DecryptData(@"C:\temp\privateKey.xml", rsaEncrypted);
                string aesSerialized = Encoding.UTF8.GetString(byteAes);

                aes = JsonSerializer.Deserialize<AesSecretDTO>(aesSerialized);
                Console.WriteLine("Keys exchanged!");
                
            }else if (message.Contains("<MESSAGE>"))
            {
                message = message.Replace("<MESSAGE>", "");

                byte[] encryptedMessage = Convert.FromBase64String(message);
                byte[] unencryptedMessage = aesEncrypter.Decrypt(encryptedMessage, aes.Key, aes.Iv);
                Console.WriteLine(Encoding.UTF8.GetString(unencryptedMessage));
            }
            else
            {
                Console.WriteLine(message);
            }
            
        }
    }
}
