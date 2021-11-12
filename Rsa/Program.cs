using CryptoService;
using Newtonsoft.Json;
using System;
using System.Text;
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
            string privateKeyPath = @"C:\DeleteLater\RsaKeys\privateKey.xml";
            string publicKeyPath = @"C:\DeleteLater\RsaKeys\publicKey.xml";
            RsaEncryption rsaEncryptionXML = new RsaEncryption();


            //Console.WriteLine("Client: " + message);
            message = message.Remove(message.Length - "<EOF>".Length);

            AesDTO aesDTO = JsonConvert.DeserializeObject<AesDTO>(message);

            byte[] toHash = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(aesDTO.Message));
            string newHash = Convert.ToBase64String(Hash.ComputeHashSha256(toHash));

            if (aesDTO.hash == newHash)
            {
                byte[] deCrypted = rsaEncryptionXML.DecryptData(privateKeyPath, aesDTO.Message.encryptedText);
                AesEncryption aesEncryption = new AesEncryption();

                string recivedMessage = Encoding.UTF8.GetString(aesEncryption.Decrypt(deCrypted, aesDTO.Message.key, aesDTO.Message.iv));
                Console.WriteLine(recivedMessage);
            }
            else
            {
                Console.WriteLine("Hash incorrect");
            }
        }
    }
}
