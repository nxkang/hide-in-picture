using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace InfoHidden.Utility
{
    public class FileTransform
    {
        public static byte[] File2ByteArray(string srcFilePath)
        {

            if (!File.Exists(srcFilePath))
            {
                throw new ArgumentException();
            }

            List<byte> fileBytes = new List<byte>();

            BinaryReader binReader = new BinaryReader(File.Open(srcFilePath, FileMode.Open));

            while (true)
            {
                try
                {
                    fileBytes.Add(binReader.ReadByte());
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }

            binReader.Close();

            return fileBytes.ToArray<byte>();

        }

        public static void ByteArray2File(string dstFilePath, byte[] dataBytes)
        {

            BinaryWriter binWriter = new BinaryWriter(File.Open(dstFilePath, FileMode.OpenOrCreate));
            for (int i = 0; i < dataBytes.Length; i++)
            {
                binWriter.Write(dataBytes[i]);
            }
            binWriter.Close();

            return;
        }

        
        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }


        public static BitmapImage ImageUri2BitmapImage(string imageUri)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(imageUri);
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.EndInit();

            return bi;
        }

    }
}
