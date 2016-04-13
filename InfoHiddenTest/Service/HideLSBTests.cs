using Microsoft.VisualStudio.TestTools.UnitTesting;
using InfoHidden.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using InfoHiddenTest.UnitTestUtilities;
using System.Windows.Media.Imaging;

namespace InfoHidden.Service.Tests
{
    [TestClass()]
    public class HideLSBTests
    {
        


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



        [TestMethod()]
        public void extractColorRedsTest()
        {
            Bitmap img = this.CreateABitmap(300, 200, 2, 1, 1);

            List<byte> colorReds = HideLSB.extractColorReds(img, 600, 1, 1);

            for (int i = 0; i < 600; i++)
            {
                Assert.AreEqual(2, colorReds[i]);
            }
        }



        [TestMethod()]
        public void HideTest()
        {
            string imageUri = PathHelper.getFilePath(@"\Data\boy.bmp");
            Bitmap imgToHide = new Bitmap(imageUri);

            byte[] bytesData = new byte[] { 211, 66, 127, 87, 56 };

            HideLSB.Hide(ref imgToHide, bytesData);
            byte[] data = HideLSB.DeHide(imgToHide);

            CollectionAssert.AreEqual(bytesData, data);
        }

        [TestMethod()]
        public void HideTest1()
        {
            string file = PathHelper.getFilePath(@"\Data\testDocx.docx");
            string coverImageUri = PathHelper.getFilePath(@"\Data\boy.bmp");
            Bitmap imgTohide = new Bitmap(coverImageUri);

            var expected = FileTransform.File2ByteArray(file);

            HideLSB.Hide(ref imgTohide, expected);
            byte[] actual = HideLSB.DeHide(imgTohide);

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void HideTest2()
        {

            Bitmap imgToHide = this.CreateABitmap(200, 300, 0, 0, 0);

            byte[] expeceted = new byte[] { 211, 66, 127, 87, 56 };

            HideLSB.Hide(ref imgToHide, expeceted);
            byte[] actual = HideLSB.DeHide(imgToHide);

            CollectionAssert.AreEqual(expeceted, actual);
        }

        [TestMethod()]
        public void HideTest3()
        {
            string file = PathHelper.getFilePath(@"\Data\testDocx.docx");
            string coverImageUri = PathHelper.getFilePath(@"\Data\boy.bmp");
            BitmapImage bitmapImgTohide = FileTransform.ImageUri2BitmapImage(coverImageUri);
            Bitmap imgTohide = FileTransform.BitmapImage2Bitmap(bitmapImgTohide);

            

            var expected = FileTransform.File2ByteArray(file);

            HideLSB.Hide(ref imgTohide, expected);
            byte[] actual = HideLSB.DeHide(imgTohide);

            CollectionAssert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void setImageColorRedsTest2()
        {

            Bitmap imgToHide = this.CreateABitmap(2, 3, 0, 0, 0);

            List<byte> expected = new List<byte>() { 1,2,3,4,5,6};

            HideLSB.setImageColorReds(ref imgToHide, 0, 0, expected);

            List<byte> actual = HideLSB.extractColorReds(imgToHide, expected.Count, 0, 0);

            CollectionAssert.AreEqual(expected, actual);
            
        }



        List<byte> extractAllReds(Bitmap img)
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



        [TestMethod()]
        public void writeFileHeadTest1()
        {
            Bitmap imgToHide = this.CreateABitmap(200, 300, 0, 0, 0);

            int expected = 260000;

            int? x = 0;
            int? y = 0;
            HideLSB.writeFileHead(ref imgToHide, expected, out x, out y);

            x = 0;
            y = 0;        
            int actual =  HideLSB.readFileHead(imgToHide, out x, out y);

            Assert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void writeFileBodyTest1()
        {
            string dataPath = PathHelper.getFilePath(@"\Data\testPng.png");
            byte[] bytesExpect = FileTransform.File2ByteArray(dataPath);

            string imagePath = PathHelper.getFilePath(@"\Data\boy.bmp");
            Bitmap image = new Bitmap(imagePath);

            HideLSB.writeFileBody(ref image, bytesExpect, 0, 0);
            byte[] bytesActual = HideLSB.readFileBody(image, bytesExpect.Length * 8, 0, 0);

            CollectionAssert.AreEqual(bytesExpect, bytesActual);
        }

        [TestMethod()]
        public void writeFileBodyTest2()
        {
            string imagePath = PathHelper.getFilePath(@"\Data\boy.bmp");
            Bitmap image = new Bitmap(imagePath);

            byte[] bytesExpect = new byte[] { 1, 2, 3, 4, 5, 6 };

            HideLSB.writeFileBody(ref image, bytesExpect, 5, 7);
            byte[] bytesActual = HideLSB.readFileBody(image, 48, 5, 7);

            CollectionAssert.AreEqual(bytesExpect, bytesActual);
        }



        [TestMethod()]
        public void Bits2BytesTest()
        {
            List<byte> bits = new List<byte>() { 1, 0, 0, 0,  0, 0, 0, 0,
                1, 1, 0, 0,  0, 0, 0, 0, };

            byte[] expected = new byte[] { 1, 3 };
            List<byte> actual = HideLSB.Bits2Bytes(bits);

            CollectionAssert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void Bytes2BitsTest()
        {
            List<byte> bits = new List<byte>() { 1, 3 };

            List<byte> expected = new List<byte>() { 1, 0, 0, 0,  0, 0, 0, 0,
                1, 1, 0, 0,  0, 0, 0, 0, };
            List<byte> actual = HideLSB.Bytes2Bits(bits);

            CollectionAssert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void Bits2IntegerTest()
        {
            // 0000 0000   0000 0000   0000 0000   0000 0100
            int data = 4;
            List<byte> expected = new List<byte>() { 0, 0, 1, 0,   0, 0, 0, 0,
                                                    0, 0, 0, 0,   0, 0, 0, 0,
                                                    0, 0, 0, 0,   0, 0, 0, 0,
                                                    0, 0, 0, 0,   0, 0, 0, 0};
            

            List<byte> actual = HideLSB.Integer2Bits(data);

            CollectionAssert.AreEqual(expected, actual);
            
        }


        [TestMethod()]
        public void Integer2BitsTest()
        {
            // 0000 0000   0000 0000   0000 0000   0000 0100
            
            List<byte> data = new List<byte>() { 0, 0, 1, 0,   0, 0, 0, 0,
                                                    0, 0, 0, 0,   0, 0, 0, 0,
                                                    0, 0, 0, 0,   0, 0, 0, 0,
                                                    0, 0, 0, 0,   0, 0, 0, 0};
            int expected = 4;

            int actual = HideLSB.Bits2Integer(data);

            Assert.AreEqual(expected, actual);

        }

        [TestMethod()]
        public void getBitsFromColorRedsLsbTest()
        {
            List<byte> colorReds = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8 };

            byte[] expected = new byte[] { 1, 0, 1, 0, 1, 0, 1, 0 };
            List<byte> actual = HideLSB.extractBitsFromColorRedsLsb(colorReds);

            CollectionAssert.AreEqual(expected, actual);
        }



    }
}