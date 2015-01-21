using System;
using System.IO;
using System.Security.Cryptography;

namespace EliteTrader
{
    public static class PasswordEncrypter
    {
        private static readonly byte[] _key = Convert.FromBase64String("vds39JOAtgjvLzyhohGRRknu0d0E7mFNSrcm5t335CA=");
        private static readonly byte[] _iv = Convert.FromBase64String("q5Q0Eb3GAoGe5SjbC5qsQg==");

        public static string Encrypt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            byte[] encryptedBytes = EncryptStringToBytes(str, _key, _iv);
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            byte[] encryptedBytes = Convert.FromBase64String(str);
            return DecryptStringFromBytes(encryptedBytes, _key, _iv);
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

    
}
