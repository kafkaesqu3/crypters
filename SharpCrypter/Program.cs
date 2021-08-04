using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace PayloadEncrypt
{
    class Program
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

        static byte[] CallAesDecryptData(byte[] ciphertext, string key)
        {
            char[] iv = @"9/\~V).A,lY&=t2b".ToCharArray();
            byte[] ret = AESDecryptData(ciphertext, Encoding.ASCII.GetBytes(key.ToCharArray()), Encoding.ASCII.GetBytes(iv));
            return ret;
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

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("ERROR: Need to pass only the path to the shell code file to encrypt.");
                Environment.Exit(1);
            }
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Could not find path to binary file: {0}", args[0]);
                Environment.Exit(1);
            }
            byte[] shellcodeBytes = File.ReadAllBytes(args[0]);
            // This is the encryption key. If changed, must also be changed in the
            // project that runs the shellcode.
            char[] password = "AD.ZIOPTISLABS.COM".ToCharArray();
            char[] iv = @"9/\~V).A,lY&=t2b".ToCharArray();

            byte[] bPwd = Encoding.ASCII.GetBytes(password);
            byte[] biv = Encoding.ASCII.GetBytes(iv);

            byte[] cipher = AesEncrypt(shellcodeBytes, bPwd, biv);

            //byte[] encShellcodeBytes = XOREncrypt(shellcodeBytes, key);

            File.WriteAllBytes(args[0] + ".encrypted", cipher);
            Console.WriteLine("Wrote encoded binary {0}.encrypted", args[0]);
            Console.Write("Here are the first 10 bytes of the cleartext (for verification purposes): ");
            int i = 0;
            while (i < 10)
            {
                Console.Write("{0:X}, ", shellcodeBytes[i]);
                i += 1;
            }
            Console.WriteLine("");
            


        }
    }
}