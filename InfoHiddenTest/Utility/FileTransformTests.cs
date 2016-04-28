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
            string path = PathHelper.GetFilePath(@"/Data/testDocx.docx");
            var bytes = FileTransform.File2ByteArray(path);
            FileTransform.ByteArray2File(PathHelper.GetFilePath("/Data/testDocx_out.docx"), bytes);
        }


        [TestMethod]
        public void File2StringTest()
        {
            string path = PathHelper.GetFilePath(@"/Data/testTxt.txt");
            string content = FileTransform.File2String(path);
            
        }
}
}