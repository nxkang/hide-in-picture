using InfoHidden.Utility;
using InfoHiddenTest.UnitTestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InfoHiddenTest.Utility
{
    [TestClass()]
    public class FileTransformTests
    {
        [TestMethod()]
        public void File2ByteArrayTest()
        {
            string srcPath = PathHelper.GetFilePath(@"/Data/testDocx.docx");
            string outPath = PathHelper.GetFilePath("/Data/testDocx_out.docx");

            var expected = FileTransform.File2ByteArray(srcPath);

            FileTransform.ByteArray2File(outPath, expected);

            var actual = FileTransform.File2ByteArray(outPath);
            File.Delete(outPath);

            CollectionAssert.AreEqual(expected, actual);
        }


        
}
}