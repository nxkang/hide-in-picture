using InfoHidden.Service;
using InfoHidden.Utility;
using InfoHiddenTest.UnitTestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfoHiddenTest.Service
{
    [TestClass()]
    public class ZIPTests
    {
        [TestMethod()]
        public void CompressTest()
        {
            string srcFilePath = PathHelper.GetFilePath(@"/Data/boy.bmp");

            var expected = FileTransform.File2ByteArray(srcFilePath);

            var zipedSrcFileBytes = Zip.Compress(expected);

            var actual = Zip.Decompress(zipedSrcFileBytes);

            CollectionAssert.AreEqual(actual, expected);
        }
    }
}