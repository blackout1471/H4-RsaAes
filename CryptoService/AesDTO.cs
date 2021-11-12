using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoService
{
    public class AesDTO
    {
        public AesDTO()
        {
            Message = new AesMessage();
        }
        public string hash;
        public AesMessage Message;

        public class AesMessage
        {
            public byte[] encryptedText;
            public byte[] key;
            public byte[] iv;
        }
    }
}
