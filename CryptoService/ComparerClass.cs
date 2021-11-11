using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoService
{
    public class ComparerClass
    {
        public bool ByteArrayComparer(byte[] byteArrayToCompare1, byte[] byteArrayToCompare2)
        {
            if (byteArrayToCompare1.Length == byteArrayToCompare2.Length)
            {
                for (int i = 0; i < byteArrayToCompare1.Length; i++)
                {
                    if (byteArrayToCompare1[i] != byteArrayToCompare2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
