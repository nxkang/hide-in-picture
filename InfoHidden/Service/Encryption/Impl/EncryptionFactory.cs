using System;

namespace InfoHidden.Service.Encryption.Impl
{
    public class EncryptionFactory
    {
        public IEncryption CreateEncryption(string name)
        {
            if ("DES".Equals(name))
                return new EncryptionDES();

            throw new InvalidOperationException();
        }
    }
}