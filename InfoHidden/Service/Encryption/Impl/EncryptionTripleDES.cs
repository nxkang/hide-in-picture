using System;
using System.Security.Cryptography;
using System.Text;

namespace InfoHidden.Service.Encryption.Impl
{
    public class EncryptionTripleDES
    {
        string TripleDESCrypto(string str, string key)
        {
            byte[] data = UnicodeEncoding.Unicode.GetBytes(str);//如果加密中文，不能用ASCII码  
            byte[] keys = ASCIIEncoding.ASCII.GetBytes(key);

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = keys;//key的长度必须为16位或24位，否则报错“指定键的大小对于此算法无效。”，des.Key不支持中文  
            des.Mode = CipherMode.ECB;//设置运算模式  
            ICryptoTransform cryp = des.CreateEncryptor();//加密  

            return Convert.ToBase64String(cryp.TransformFinalBlock(data, 0, data.Length));
        }

        string TripleDESCryptoDe(string str, string key)
        {
            byte[] data = Convert.FromBase64String(str);
            byte[] keys = ASCIIEncoding.ASCII.GetBytes(key);

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = keys;
            des.Mode = CipherMode.ECB;//设置运算模式  
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cryp = des.CreateDecryptor();//解密  

            return UnicodeEncoding.Unicode.GetString(cryp.TransformFinalBlock(data, 0, data.Length));
        }
    }
}
