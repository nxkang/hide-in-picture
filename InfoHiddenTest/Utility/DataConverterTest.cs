using System.Collections.Generic;
using InfoHidden.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoHiddenTest.Utility
{

    [TestClass()]
    public class DataConverterTest
    {
        #region Bits2ByteTest

        [TestMethod()]
        public void Bits2ByteTest()
        {
            List<byte> bits = new List<byte>() {0,0,0,0, 0,1,0,0};
            byte expected = 32;

            byte actual = DataConverter.Bits2Byte(bits);

            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void Byte2BitsTest()
        {
            byte aByte = 32;

            List<byte> expected = new List<byte>() { 0, 0, 0, 0,  0, 1, 0, 0 };

            List<byte> actual = DataConverter.Byte2Bits(aByte);

            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion


        #region Bits2BytesTest

        [TestMethod()]
        public void Bits2BytesTest()
        {
            List<byte> bits = new List<byte>() { 1, 0, 0, 0,  0, 0, 0, 0,
                1, 1, 0, 0,  0, 0, 0, 0, };

            byte[] expected = new byte[] { 1, 3 };
            List<byte> actual = DataConverter.Bits2Bytes(bits);

            CollectionAssert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void Bytes2BitsTest()
        {
            List<byte> bits = new List<byte>() { 1, 3 };

            List<byte> expected = new List<byte>() { 1, 0, 0, 0,  0, 0, 0, 0,
                1, 1, 0, 0,  0, 0, 0, 0, };
            List<byte> actual = DataConverter.Bytes2Bits(bits);

            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion


        #region Bits2IntegerTest

        [TestMethod()]
        public void Bits2IntegerTest()
        {
            // 0000 0000   0000 0000   0000 0000   0000 0100
            int data = 4;
            List<byte> expected = new List<byte>() { 0, 0, 1, 0,   0, 0, 0, 0,
                0, 0, 0, 0,   0, 0, 0, 0,
                0, 0, 0, 0,   0, 0, 0, 0,
                0, 0, 0, 0,   0, 0, 0, 0};
            

            List<byte> actual = DataConverter.Integer2Bits(data);

            CollectionAssert.AreEqual(expected, actual);
            
        }


        [TestMethod()]
        public void Integer2BitsTest()
        {
            // 0000 0000   0000 0000   0000 0000   0000 0100
            
            List<byte> data = new List<byte>() { 0, 0, 1, 0,   0, 0, 0, 0,
                0, 0, 0, 0,   0, 0, 0, 0,
                0, 0, 0, 0,   0, 0, 0, 0,
                0, 0, 0, 0,   0, 0, 0, 0};
            int expected = 4;

            int actual = DataConverter.Bits2Integer(data);

            Assert.AreEqual(expected, actual);

        }

        #endregion


        #region Bytes2UIntTest

        [TestMethod()]
        public void Bytes2UInt()
        {
            byte[] bytes = new byte[] {1,0,0,0};

            uint expected = 16777216;

            uint actual = DataConverter.Bytes2UInt(bytes);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void UInt2Bytes()
        {
            uint aUInt = 16777216;

            byte[] expected = new byte[] { 1, 0, 0, 0 };

            byte[] actual = DataConverter.UInt2Bytes(aUInt);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void UInt2ByteArrayTest()
        {
            uint v = 5;
            byte[] actual = DataConverter.UInt2Bytes(v);
            uint a = DataConverter.Bytes2UInt(actual);
            Assert.AreEqual(a, (uint)5);
        }

        #endregion


        #region UInts2BytesTest

        [TestMethod()]
        public void UInts2BytesTest()
        {
            uint[] uints = new uint[] { 16777216, 1};

            byte[] expected = new byte[] {1,0,0,0,
                0,0,0,1};
            byte[] actual = DataConverter.UInts2Bytes(uints);

            CollectionAssert.AreEqual(expected, actual);

        }

        #endregion


        #region Bytes2StringTest

        [TestMethod()]
        public void Bytes2StringTest()
        {
            byte[] bytes = new byte[] {97, 98, 99, 100, 101};

            string expected = "abcde";

            string acutal = DataConverter.Bytes2String(bytes);

            Assert.AreEqual(expected, acutal);
        }

        [TestMethod()]
        public void String2BytesTest()
        {
            string str = "abcde";

            byte[] expected = new byte[] { 97, 98, 99, 100, 101 };

            byte[] actual = DataConverter.String2Bytes(str);

            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion


        #region GetOneBitFrom

        [TestMethod()]
        public void GetOneBitFromIntegerTest()
        {
            int anInteger = 2;

            int expected = 1;

            int acutal = DataConverter.GetOneBitFromInteger(anInteger, 1);

            Assert.AreEqual(expected, acutal);
        }

        [TestMethod()]
        public void GetOneBitFromBitTest()
        {
            int aByte = 2;

            int expected = 1;

            int acutal = DataConverter.GetOneBitFromInteger(aByte, 1);

            Assert.AreEqual(expected, acutal);
        }

        #endregion
    }
}