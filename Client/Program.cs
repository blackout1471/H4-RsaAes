using System;
using System.Text;
using Newtonsoft.Json;
using CryptoService;
using System.Security.Cryptography;

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

                //Console.Write("Path to public key: ");
                //string publicKey = Console.ReadLine(); ; // geefs public

                string publicKey = @"C:\DeleteLater\RsaKeys\publicKey.xml"; // geefs public

                bool sendingData = true;
                while (sendingData)
                {
                    Console.Write("Text to send: ");
                    string text = Console.ReadLine();

                    if (text.ToLower() == "exit")
                        sendingData = false;

                    byte[] key = aesEncryption.GenerateRandomNumber(32);
                    byte[] iv = aesEncryption.GenerateRandomNumber(16);

                    byte[] Aesencrypted = aesEncryption.Encrypt(Encoding.UTF8.GetBytes(text), key, iv);
        

                    //string privateKey = @"C:\DeleteLater\RsaKeys\privateKey.xml";
                    //rsaEncryption.AssignNewKeyXml(publicKey, privateKey);

                    byte[] rsaEncrypted = rsaEncryption.EncryptData(publicKey, Aesencrypted);

                    AesDTO aesDTO = new AesDTO();
                    aesDTO.Message.encryptedText = rsaEncrypted;
                    aesDTO.Message.key = key;
                    aesDTO.Message.iv = iv;
                    string hashedMessage = JsonConvert.SerializeObject(aesDTO.Message);

                    aesDTO.hash = Convert.ToBase64String(Hash.ComputeHashSha256(Encoding.UTF8.GetBytes(hashedMessage)));

                    string fullMessage = JsonConvert.SerializeObject(aesDTO) + "<EOF>";

                    //Console.WriteLine(Encoding.UTF8.GetBytes(textToSend).Length);
                    //socketConnection.DisConnenect();
                    //Console.WriteLine(textToSend);
                    socketConnection.SendMessage(text);
                    Console.WriteLine("Sended");
                    Console.WriteLine("---------------------------");
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
