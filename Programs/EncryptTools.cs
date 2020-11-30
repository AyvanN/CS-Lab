using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace FileWatcher
{
    class EncryptTools
    {
        Rijndael Encrypter;

        public EncryptTools()
        {
            Encrypter = Rijndael.Create();
            Encrypter.Padding = PaddingMode.None;
        }

        public void DecryptFile(string pathFile, string pathDecrypt)
        {
            using (FileStream file = new FileStream(pathFile, FileMode.Open))
            {
                using (FileStream stream = new FileStream(pathDecrypt, FileMode.Create, FileAccess.Write))
                {
                    ICryptoTransform decryptor = Encrypter.CreateDecryptor(Encrypter.Key, Encrypter.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(file, decryptor, CryptoStreamMode.Read))
                    {
                        csDecrypt.CopyTo(stream);
                    }
                }
            }
        }

        public void EncryptFile(string pathFile, string pathEncrypt)
        {
            using (FileStream file = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
            {
                using (FileStream stream = new FileStream(pathEncrypt, FileMode.Create))
                {
                    using (Rijndael rijAlg = Rijndael.Create())
                    {

                        ICryptoTransform encryptor = Encrypter.CreateEncryptor(Encrypter.Key, Encrypter.IV);


                        using (MemoryStream msEncrypt = new MemoryStream())
                        {
                            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            {
                                file.CopyTo(csEncrypt);
                                stream.Write(msEncrypt.ToArray(), 0, msEncrypt.ToArray().Length);
                            }
                        }
                    }
                }
            }
        }
    }
}
