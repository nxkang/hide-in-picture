using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InfoHiddenTest.UnitTestUtilities
{
    public static class PathHelper
    {

        public static string getFilePath(string relativePath)
        {
            string currentDirectoryPath = Environment.CurrentDirectory;
            string projectRootPath = currentDirectoryPath.Substring(0, currentDirectoryPath.Length - 10);
            string absolutePath = projectRootPath + relativePath;

            if (!File.Exists(absolutePath))
                throw new ArgumentException();

            return absolutePath;
        }
    }
}
