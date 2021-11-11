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
            SocketConnection socketConnection = new SocketConnection("192.168.1.110", 5500);
            socketConnection.Connect();

            string text = "Hello world";
            AesEncryption aesEncryption = new AesEncryption();
            byte[] key = aesEncryption.GenerateRandomNumber(32);
            byte[] iv = aesEncryption.GenerateRandomNumber(16);

            byte[] Aesencrypted = aesEncryption.Encrypt(Encoding.UTF8.GetBytes(text), key, iv);

            RsaEncryption rsaEncryption = new RsaEncryption();
            string publicKey = @"C:\DeleteLater\RsaKeys\publicKey.xml"; // geefs public
            //string privateKey = @"C:\DeleteLater\RsaKeys\privateKey.xml";
            //rsaEncryption.AssignNewKeyXml(publicKey, privateKey);

            byte[] rsaEncrypted = rsaEncryption.EncryptData(publicKey, Aesencrypted);


            AesDTO aesDTO = new AesDTO();
            aesDTO.encryptedText = rsaEncrypted;
            aesDTO.key = key;
            aesDTO.iv = iv;
            string textToSend = JsonConvert.SerializeObject(aesDTO) + "<EOF>";

            string hash = "";
            using (var sha256 = SHA256.Create())
            {
                hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(textToSend)));
            }
            aesDTO.hash = hash;

            textToSend = JsonConvert.SerializeObject(aesDTO) + "<EOF>";

            //Console.WriteLine(Encoding.UTF8.GetBytes(textToSend).Length);
            //socketConnection.DisConnenect();
            //Console.WriteLine(textToSend);
            socketConnection.SendMessage(textToSend);

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
