using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfoHidden.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoHidden.Service.Tests
{
    [TestClass()]
    public class ZIPTests
    {
        [TestMethod()]
        public void CompressTest()
        {
            string srcFilePath = @"D:/Users/liukang/documents/visual studio 2010/Projects/InfoHidden/InfoHidden/Data/boy.bmp";
            string outFilePath = @"D:/Users/liukang/documents/visual studio 2010/Projects/InfoHidden/InfoHidden/Data/boy2.bmp";

            var srcFileBytes = FileTransform.File2ByteArray(srcFilePath);

            var zipedSrcFileBytes = ZIP.Compress(srcFileBytes);
            var unzipedSrcFileBytes = ZIP.Decompress(zipedSrcFileBytes);

            FileTransform.ByteArray2File(outFilePath, unzipedSrcFileBytes);
        }
    }
}