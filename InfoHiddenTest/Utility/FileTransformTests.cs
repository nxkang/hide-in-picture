using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfoHidden.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoHidden.Service.Tests
{
    [TestClass()]
    public class FileTransformTests
    {
        [TestMethod()]
        public void File2ByteArrayTest()
        {
            string path = @"D:/Users/liukang/documents/visual studio 2010/Projects/InfoHidden/InfoHiddenTest/Data/testDocx.docx";
            var bytes = FileTransform.File2ByteArray(path);
            FileTransform.ByteArray2File(@"D:/Users/liukang/documents/visual studio 2010/Projects/InfoHidden/InfoHiddenTest/Data/output_kms8.log", bytes);
        }


    }
}