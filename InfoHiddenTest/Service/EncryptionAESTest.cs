using InfoHidden.Service.Encryption.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoHiddenTest.Service
{
    [TestClass()]
    public class EncryptionAESTest
    {
        [TestMethod()]
        public void EncipherTest()
        {
            uint[] keys =
            {
                12, 23, 34, 45, 56, 67, 78, 89,
            };

            byte[] expected = new byte[] { 96, 97, 98, 99 };

            var encrypter = new EncryptionAES();

            byte[] ciphertext = encrypter.Encrypt(expected, keys);
            byte[] actual = encrypter.Decrypt(ciphertext, keys);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}