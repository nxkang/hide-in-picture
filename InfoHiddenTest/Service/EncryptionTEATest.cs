using InfoHidden.Service.Encryption.Impl;
using InfoHidden.Utility;
using InfoHiddenTest.UnitTestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoHiddenTest.Service
{
    
    
    /// <summary>
    ///这是 EncryptionTEATest 的测试类，旨在
    ///包含所有 EncryptionTEATest 单元测试
    ///</summary>
    [TestClass()]
    public class EncryptionTEATest
    {


        private TestContext _testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Encrypt 的测试
        ///</summary>
        [TestMethod()]
        public void EncryptTest()
        {
            uint[] keys =
            {
                12, 23, 34, 45
            };

            byte[] expected = new byte[] { 96, 97, 98, 99 };

            var encrypter = new EncryptionTEA();

            byte[] ciphertext = encrypter.Encrypt(expected, keys);
            byte[] actual = encrypter.Decrypt(ciphertext, keys);

            CollectionAssert.AreEqual(expected, actual);
        }


        /// <summary>
        ///Encipher 的测试
        ///</summary>
        [TestMethod()]
        public void EncipherTest()
        {
            uint[] v = new uint[2]{3, 4}; 
            uint[] key = new uint[4]{1,2,3,4}; 
            EncryptionTEA.Encipher(v, key);
            EncryptionTEA.Deciper(v, key);

            CollectionAssert.AreEqual(v, new uint[2]{3, 4});
        }

        
    }
}
