using System;
using System.Text;
using CryptoService;
using System.Security.Cryptography;
using System.Text.Json;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketConnection socketConnection = null;
            try
            {
                Console.Write("Insert ip: ");
                string ipAdress = Console.ReadLine();

                Console.Write("Insert port: ");
                int port = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine($"Connecting to {ipAdress}:{port}");
                socketConnection = new SocketConnection(ipAdress, port);
                socketConnection.Connect();
                Console.WriteLine("Connected");

                AesEncryption aesEncryption = new AesEncryption();
                RsaEncryption rsaEncryption = new RsaEncryption();

                byte[] key = aesEncryption.GenerateRandomNumber(32);
                byte[] iv = aesEncryption.GenerateRandomNumber(16);
                AesSecretDTO aesSecretDTO = new AesSecretDTO();
                aesSecretDTO.Key = key;
                aesSecretDTO.Iv = iv;
                string aesSecretObject = JsonSerializer.Serialize(aesSecretDTO);
                string publicKey = @"C:\temp\publicKey.xml"; // geefs public
                byte[] rsaEncrypted = rsaEncryption.EncryptData(publicKey, Encoding.UTF8.GetBytes(aesSecretObject));
                string data = Convert.ToBase64String(rsaEncrypted);
                socketConnection.SendMessage(data + "<KEY><EOF>");


                bool sendingData = true;
                while (sendingData)
                {
                    Console.Write("Text to send: ");
                    string text = Console.ReadLine();


                    byte[] encryptedText = aesEncryption.Encrypt(Encoding.UTF8.GetBytes(text), key, iv);
                    socketConnection.SendMessage(Convert.ToBase64String(encryptedText) + "<MESSAGE><EOF>");
                }
            }
            catch (Exception e)
            {
                if (socketConnection != null)
                    socketConnection.DisConnenect();
                Console.WriteLine("Error: " + e);
            }

            if (socketConnection != null)
                socketConnection.DisConnenect();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
