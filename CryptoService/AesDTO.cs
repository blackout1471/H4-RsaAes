using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoService
{
    public class AesDTO
    {
        public byte[] encryptedText;
        public byte[] key;
        public byte[] iv;
    }
}
