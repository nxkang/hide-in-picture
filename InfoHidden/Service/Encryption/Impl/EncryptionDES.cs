using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using InfoHidden.Utility;

namespace InfoHidden.Service.Encryption.Impl
{
    public class EncryptionDES : IEncryption
    {
        private string Encrypter(string str, byte[] keys, byte[] ivs)
        {
            //加密  
            byte[] strs = Encoding.Unicode.GetBytes(str);


            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();

            ICryptoTransform transform = desc.CreateEncryptor(keys, ivs);//加密对象  
            CryptoStream cStream = new CryptoStream(mStream, transform, CryptoStreamMode.Write);
            cStream.Write(strs, 0, strs.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        private string Decrypter(string str, byte[] keys, byte[] ivs)
        {
            //解密  
            byte[] strs = Convert.FromBase64String(str);

            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();

            ICryptoTransform transform = desc.CreateDecryptor(keys, ivs);//解密对象  

            CryptoStream cStream = new CryptoStream(mStream, transform, CryptoStreamMode.Write);
            cStream.Write(strs, 0, strs.Length);
            cStream.FlushFinalBlock();
            return Encoding.Unicode.GetString(mStream.ToArray());
        }


        public byte[] Encrypt(byte[] plaintext, uint[] key)
        {
            byte[] ivs = new byte[]{0, 0, 0, 0};

            string ciphertext = Encrypter(DataConverter.Bytes2String(plaintext), DataConverter.UInts2Bytes(key), ivs);

            return DataConverter.String2Bytes(ciphertext);
        }

        public byte[] Decrypt(byte[] ciphertext, uint[] key)
        {
            byte[] ivs = new byte[] { 0, 0, 0, 0 };

            string plaintxt = Decrypter(DataConverter.Bytes2String(ciphertext), DataConverter.UInts2Bytes(key), ivs);

            return DataConverter.String2Bytes(plaintxt);
        }


    }
}
