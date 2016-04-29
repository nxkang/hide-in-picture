using System;

namespace InfoHidden.Service.Encryption.Impl
{
    public class EncryptionFactory
    {
        public static IEncryption CreateEncryption(string name)
        {
            if ("DES".Equals(name))
                return new EncryptionDES();

            if ("AES".Equals(name))
                return new EncryptionAES();

            if ("Rijndael".Equals(name))
                return  new EncryptionRijndael();

            if("TEA".Equals(name))
                return  new EncryptionTEA();

            throw new InvalidOperationException();
        }
    }
}