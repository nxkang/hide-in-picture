using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfoHidden.Service.Encryption.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoHiddenTest.Service
{
    [TestClass()]
    public class EncryptionDESTest
    {

        [TestMethod()]
        public void EncipherTest()
        {
            uint[] key = new uint[] { 1, 2, 3, 4, 5, 6};

            byte[] expected = new byte[] {96, 97, 98, 99};

            var encrypter = new  EncryptionDES();

            byte[] ciphertext = encrypter.Encrypt(expected, key);
            byte[] actual = encrypter.Decrypt(ciphertext, key);

            CollectionAssert.AreEqual(expected, actual);
        }


    }
}
