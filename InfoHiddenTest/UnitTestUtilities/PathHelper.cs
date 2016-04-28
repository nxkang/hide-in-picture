using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InfoHiddenTest.UnitTestUtilities
{
    public static class PathHelper
    {

        public static string GetFilePath(string relativePath)
        {
            string currentDirectoryPath = Environment.CurrentDirectory;
            string projectRootPath = currentDirectoryPath.Substring(0, currentDirectoryPath.Length - 10);
            string absolutePath = projectRootPath + relativePath;

            return absolutePath;
        }
    }
}
