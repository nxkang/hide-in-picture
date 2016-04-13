using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace InfoHidden.Utility
{
    public static class SerializeHelper
    {
        public static void Serialze(object obj, string filePath, string fileName)
        {
            
            var fs = File.Open(filePath + "/" + fileName, FileMode.OpenOrCreate);
            BinaryFormatter binFormat = new BinaryFormatter();
            binFormat.Serialize(fs, obj);
            fs.Close();
        }

        public static object DeSerialize(string filePath, string fileName)
        {
            var fs = File.Open(filePath + "/" + fileName, FileMode.Open);
            BinaryFormatter binFormat = new BinaryFormatter();
            Object obj = binFormat.Deserialize(fs);
            fs.Close();
            return obj;
        }
    }
}
