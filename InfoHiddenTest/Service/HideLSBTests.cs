#region

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;
using InfoHidden.Service;
using InfoHidden.Utility;
using InfoHiddenTest.UnitTestUtilities;
using InfoHiddenTest.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace InfoHiddenTest.Service
{
    [TestClass]
    public class HideLSBTests
    {
        private readonly DataConverterTest _dataConverterTest = new DataConverterTest();

        private Bitmap CreateABitmap(int width, int height, byte red, byte green, byte blue)
        {
            Bitmap img = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color currentPixel = Color.FromArgb(red, green, blue);
                    img.SetPixel(x, y, currentPixel);
                }
            }

            return img;
        }


        [TestMethod]
        public void ExtractColorRedsTest()
        {
            Bitmap img = CreateABitmap(300, 200, 2, 1, 1);

            List<byte> colorReds = HideLSB.ExtractColorReds(img, 600, 1, 1);

            for (int i = 0; i < 600; i++)
            {
                Assert.AreEqual(2, colorReds[i]);
            }
        }


        [TestMethod]
        public void HideTest()
        {
            string imageUri = PathHelper.GetFilePath(@"\Data\boy.bmp");
            Bitmap imgToHide = new Bitmap(imageUri);

            byte[] bytesData = {211, 66, 127, 87, 56};

            HideLSB.Hide(ref imgToHide, bytesData);
            byte[] data = HideLSB.DeHide(imgToHide);

            CollectionAssert.AreEqual(bytesData, data);
        }

        [TestMethod]
        public void HideTest1()
        {
            string file = PathHelper.GetFilePath(@"\Data\testDocx.docx");
            string coverImageUri = PathHelper.GetFilePath(@"\Data\boy.bmp");
            Bitmap imgTohide = new Bitmap(coverImageUri);

            var expected = FileTransform.File2ByteArray(file);

            HideLSB.Hide(ref imgTohide, expected);
            byte[] actual = HideLSB.DeHide(imgTohide);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void HideTest2()
        {
            Bitmap imgToHide = CreateABitmap(200, 300, 0, 0, 0);

            byte[] expeceted = {211, 66, 127, 87, 56};

            HideLSB.Hide(ref imgToHide, expeceted);
            byte[] actual = HideLSB.DeHide(imgToHide);

            CollectionAssert.AreEqual(expeceted, actual);
        }

        [TestMethod]
        public void HideTest3()
        {
            string file = PathHelper.GetFilePath(@"\Data\testDocx.docx");
            string coverImageUri = PathHelper.GetFilePath(@"\Data\boy.bmp");
            BitmapImage bitmapImgTohide = FileTransform.ImageUri2BitmapImage(coverImageUri);
            Bitmap imgTohide = FileTransform.BitmapImage2Bitmap(bitmapImgTohide);


            var expected = FileTransform.File2ByteArray(file);

            HideLSB.Hide(ref imgTohide, expected);
            byte[] actual = HideLSB.DeHide(imgTohide);

            CollectionAssert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void SetImageColorRedsTest2()
        {
            Bitmap imgToHide = CreateABitmap(2, 3, 0, 0, 0);

            List<byte> expected = new List<byte> {1, 2, 3, 4, 5, 6};

            HideLSB.SetImageColorReds(ref imgToHide, 0, 0, expected);

            List<byte> actual = HideLSB.ExtractColorReds(imgToHide, expected.Count, 0, 0);

            CollectionAssert.AreEqual(expected, actual);
        }


        List<byte> ExtractAllReds(Bitmap img)
        {
            List<byte> allReds = new List<byte>();
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    allReds.Add(img.GetPixel(x, y).R);
                }
            }
            return allReds;
        }


        [TestMethod]
        public void WriteFileHeadTest1()
        {
            Bitmap imgToHide = CreateABitmap(200, 300, 0, 0, 0);

            int expected = 260000;

            int? x = 0;
            int? y = 0;
            HideLSB.WriteFileHead(ref imgToHide, expected, out x, out y);

            x = 0;
            y = 0;
            int actual = HideLSB.ReadFileHead(imgToHide, out x, out y);

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void WriteFileBodyTest1()
        {
            string dataPath = PathHelper.GetFilePath(@"\Data\testPng.png");
            byte[] bytesExpect = FileTransform.File2ByteArray(dataPath);

            string imagePath = PathHelper.GetFilePath(@"\Data\boy.bmp");
            Bitmap image = new Bitmap(imagePath);

            HideLSB.WriteFileBody(ref image, bytesExpect, 0, 0);
            byte[] bytesActual = HideLSB.ReadFileBody(image, bytesExpect.Length*8, 0, 0);

            CollectionAssert.AreEqual(bytesExpect, bytesActual);
        }

        [TestMethod]
        public void WriteFileBodyTest2()
        {
            string imagePath = PathHelper.GetFilePath(@"\Data\boy.bmp");
            Bitmap image = new Bitmap(imagePath);

            byte[] bytesExpect = {1, 2, 3, 4, 5, 6};

            HideLSB.WriteFileBody(ref image, bytesExpect, 5, 7);
            byte[] bytesActual = HideLSB.ReadFileBody(image, 48, 5, 7);

            CollectionAssert.AreEqual(bytesExpect, bytesActual);
        }


        [TestMethod]
        public void GetBitsFromColorRedsLsbTest()
        {
            List<byte> colorReds = new List<byte> {1, 2, 3, 4, 5, 6, 7, 8};

            byte[] expected = {1, 0, 1, 0, 1, 0, 1, 0};
            List<byte> actual = HideLSB.ExtractBitsFromColorRedsLsb(colorReds);

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}