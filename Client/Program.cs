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

                byte[] key = aesEncryption.GenerateRandomNumber(32);
                byte[] iv = aesEncryption.GenerateRandomNumber(16);
                AesSecretDTO aesSecretDTO = new AesSecretDTO();
                aesSecretDTO.key = key;
                aesSecretDTO.iv = iv;
                string aesSecretObject = JsonConvert.SerializeObject(aesSecretDTO);
                string publicKey = @"C:\DeleteLater\RsaKeys\publicKey.xml"; // geefs public
                byte[] rsaEncrypted = rsaEncryption.EncryptData(publicKey, Encoding.UTF8.GetBytes(aesSecretObject));
                socketConnection.SendMessage(Convert.ToBase64String(rsaEncrypted) + "<KEY><EOF>");
                //Console.Write("Path to public key: ");
                //string publicKey = Console.ReadLine(); ; // geefs public


                bool sendingData = true;
                while (sendingData)
                {
                    Console.Write("Text to send: ");
                    string text = Console.ReadLine();

                    if (text.ToLower() == "exit")
                        sendingData = false;


                    byte[] Aesencrypted = aesEncryption.Encrypt(Encoding.UTF8.GetBytes(text), key, iv);


                    //string privateKey = @"C:\DeleteLater\RsaKeys\privateKey.xml";
                    //rsaEncryption.AssignNewKeyXml(publicKey, privateKey);


                    AesMessageDTO aesMessageDTO = new AesMessageDTO();
                    aesMessageDTO.encryptedText = Aesencrypted;
                   // string hashedMessage = JsonConvert.SerializeObject(aesMessageDTO.encryptedText);

                    aesMessageDTO.hash = Convert.ToBase64String(Hash.ComputeHashSha256(Encoding.UTF8.GetBytes(text)));

                    string aesMessageObject = JsonConvert.SerializeObject(aesMessageDTO);
                    socketConnection.SendMessage(aesMessageObject + "<MESSAGE><EOF>");

                    //Console.WriteLine(Encoding.UTF8.GetBytes(textToSend).Length);
                    //socketConnection.DisConnenect();
                    //Console.WriteLine(textToSend);
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
