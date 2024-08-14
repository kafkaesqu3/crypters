using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SharpCrypter
{
    class Legacy
    {
        // Simple XOR routine
        static byte[] XorEncrypt(byte[] origBytes, char[] cryptor)
        {
            byte[] result = new byte[origBytes.Length];
            int j = 0;
            for (int i = 0; i < origBytes.Length; i++)
            {
                // If we're at the end of the encryption key, move
                // pointer back to beginning.
                if (j == cryptor.Length - 1)
                {
                    j = 0;
                }
                // Perform the XOR operation
                byte res = (byte)(origBytes[i] ^ Convert.ToByte(cryptor[j]));
                // Store the result
                result[i] = res;
                // Increment the pointer of the XOR key
                j += 1;
            }
            // Return results
            return result;
        }

        static public byte[] AesEncrypt(byte[] plain, byte[] pwd, byte[] iv)
        {
            byte[] cipherBytes = null;

            var key = new Rfc2898DeriveBytes(pwd, iv, 32768);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                aesAlg.Padding = PaddingMode.Zeros;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plain, 0, plain.Length);
                        csEncrypt.Close();
                    }
                    cipherBytes = msEncrypt.ToArray();
                }
            }
            return cipherBytes;
        }

        static public byte[] AESDecryptData(byte[] cipher, byte[] pwd, byte[] iv)
        {
            byte[] plainBytes = null;

            var key = new Rfc2898DeriveBytes(pwd, iv, 32768);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                aesAlg.Padding = PaddingMode.Zeros;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipher))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(cipher, 0, cipher.Length);
                        csDecrypt.Close();
                    }
                    plainBytes = msDecrypt.ToArray();
                }
            }
            return plainBytes;
        }
    }

}
