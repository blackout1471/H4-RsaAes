namespace CryptoService
{
    public class AesMessageDTO
    {
        public byte[] EncryptedText { get; set; }
        public string Hash { get; set; }
    }
}
