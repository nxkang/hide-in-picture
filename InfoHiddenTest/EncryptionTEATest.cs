using InfoHidden.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InfoHiddenTest
{
    
    
    /// <summary>
    ///这是 EncryptionTEATest 的测试类，旨在
    ///包含所有 EncryptionTEATest 单元测试
    ///</summary>
    [TestClass()]
    public class EncryptionTEATest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
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
             byte[] plaintext = FileTransform.File2ByteArray("D:/Users/liukang/documents/visual studio 2010/Projects/InfoHidden/InfoHidden/Data/TextFile1.jpg");
             uint[] key = new uint[4] { 1, 2, 3, 4 }; 
            
             byte[] actual = EncryptionTEA.Encrypt(plaintext, key);
             byte[] plain = EncryptionTEA.Decrypt(actual, key);

            FileTransform.ByteArray2File("D:/Users/liukang/documents/visual studio 2010/Projects/InfoHidden/InfoHidden/Data/TextFile2.jpg", plain);
        }


        /// <summary>
        ///paddingData 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("InfoHidden.exe")]
        public void paddingDataTest()
        {
            byte[] originData = new byte[]{ 1, 2, 3, 4 }; 
            byte[] expected = new byte[]{4, 0, 0, 0, 1, 2, 3, 4}; 
            byte[] actual;
            actual = EncryptionTEA_Accessor.paddingData(originData);
            
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///depaddingData 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("InfoHidden.exe")]
        public void depaddingDataTest()
        {
            byte[] originData = new byte[] { 4, 0, 0, 0, 1, 2, 3, 4 };
            byte[] expected = new byte[] {1, 2, 3, 4 }; 
            byte[] actual;
            actual = EncryptionTEA_Accessor.depaddingData(originData);
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///File2ByteArray 的测试
        ///</summary>
        [TestMethod()]
        public void File2ByteArrayTest()
        {
            string srcFilePath = "D:/Users/liukang/documents/visual studio 2010/Projects/InfoHidden/InfoHidden/Data/TextFile1.txt";
            string dstFilePath = "D:/Users/liukang/documents/visual studio 2010/Projects/InfoHidden/InfoHidden/Data/TextFile2.txt"; 
            
            byte[] actual = FileTransform.File2ByteArray(srcFilePath);

            FileTransform.ByteArray2File(dstFilePath, actual); 
        }


        /// <summary>
        ///encipher 的测试
        ///</summary>
        [TestMethod()]
        public void encipherTest()
        {
            uint[] v = new uint[2]{3, 4}; 
            uint[] key = new uint[4]{1,2,3,4}; 
            EncryptionTEA.encipher(v, key);
            EncryptionTEA.deciper(v, key);

            CollectionAssert.AreEqual(v, new uint[2]{3, 4});
        }

        /// <summary>
        ///UInt2ByteArray 的测试
        ///</summary>
        [TestMethod()]
        [DeploymentItem("InfoHidden.exe")]
        public void UInt2ByteArrayTest()
        {
            uint v = 5; 
            byte[] actual = EncryptionTEA.UInt2ByteArray(v);
            uint a = EncryptionTEA.ByteArray2UInt(actual);
            Assert.AreEqual(a, (uint)5);
        }
    }
}
