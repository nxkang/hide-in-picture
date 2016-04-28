using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using InfoHidden.Service.ServiceException;
using System.Diagnostics;
using InfoHidden.Utility;

namespace InfoHidden.Service
{
    public class HideLSB
    {
        #region CONSTANT

        const int FILE_HEAD_BIT_LENGTH = 32;

        #endregion

        #region public methods

        public static void Erase(ref Bitmap image)
        {
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color currentPixel = image.GetPixel(x, y);
                    byte randomRed = MakeNewRed(currentPixel.R, new Random().Next(2));
                    Color randomColor = Color.FromArgb(randomRed, currentPixel.G, currentPixel.B);
                    image.SetPixel(x, y, randomColor);
                }
            }
        }

        public static void Hide(ref Bitmap image, byte[] data)
        {
            int imageBitDataCapacity = image.Width*image.Height;
            int dataBitLength = data.Length*8;

            if (dataBitLength + FILE_HEAD_BIT_LENGTH > imageBitDataCapacity)
            {
                throw new ImageHideCapacityTooSmallException();
            }

            int? x = null;
            int? y = null;

            WriteFileHead(ref image, dataBitLength, out x, out y);

            WriteFileBody(ref image, data, (int) x, (int) y);
        }

        public static byte[] DeHide(Bitmap image)
        {
            int? x = null;
            int? y = null;

            int dataBitLength = ReadFileHead(image, out x, out y);

            Debug.Assert(dataBitLength%8 == 0);

            byte[] data = ReadFileBody(image, dataBitLength, (int) x, (int) y);

            return data;
        }

        #endregion

        #region private methods

        public static void WriteFileHead(ref Bitmap image, int dataBitLength, out int? x, out int? y)
        {
            x = 0;
            y = 0;
            List<byte> bitsData = DataConverter.Integer2Bits(dataBitLength);
            byte[] data = DataConverter.Bits2Bytes(bitsData).ToArray();
            WriteFileBody(ref image, data, (int) x, (int) y);
            x = 0;
            y = 32;
        }

        public static int ReadFileHead(Bitmap image, out int? x, out int? y)
        {
            x = 0;
            y = 0;
            List<byte> retBits = new List<byte>(ReadFileBody(image, 32, (int) x, (int) y));
            int ret = DataConverter.Bits2Integer(DataConverter.Bytes2Bits(retBits));
            x = 0;
            y = 32;
            return ret;
        }

        public static void WriteFileBody(ref Bitmap image, byte[] data, int x, int y)
        {
            List<byte> originColorReds = ExtractColorReds(image, data.Length*8, x, y);

            List<byte> bitsData = DataConverter.Bytes2Bits(new List<byte>(data));
            int dataBitLength = data.Length*8;
            List<byte> colorReds = ExtractColorReds(image, dataBitLength, x, y);
            List<byte> newColorReds = MakeNewReds(colorReds, bitsData);
            SetImageColorReds(ref image, x, y, newColorReds);
        }

        public static byte[] ReadFileBody(Bitmap image, int dataBitLength, int x, int y)
        {
            try
            {
                List<byte> colorReds = ExtractColorReds(image, dataBitLength, x, y);
                List<byte> bits = ExtractBitsFromColorRedsLsb(colorReds);
                List<byte> bytes = DataConverter.Bits2Bytes(bits);

                return bytes.ToArray();
            }
            catch (Exception)
            {
                throw new PasswordWrongException();
            }
        }


        private static byte ReadLsbBitFromColorR(Color color)
        {
            return (byte) (color.R & 0x01);
        }

        public static List<byte> ExtractColorReds(Bitmap image, int length, int x, int y)
        {
            List<byte> colorReds = new List<byte>(length);

            for (; x < image.Width; x++)
            {
                for (; y < image.Height; y++)
                {
                    colorReds.Add(image.GetPixel(x, y).R);

                    if (colorReds.Count >= length)
                    {
                        return colorReds;
                    }
                }
                y = 0;
            }

            throw new InvalidOperationException();
        }

        public static List<byte> ExtractBitsFromColorRedsLsb(List<byte> colorReds)
        {
            List<byte> bits = new List<byte>(colorReds.Count);
            for (int i = 0; i < colorReds.Count; i++)
            {
                bits.Add((byte) DataConverter.GetOneBitFromByte(colorReds[i], 0));
            }

            return bits;
        }


        private static byte MakeNewRed(byte oldR, int aBit)
        {
            int lsbR = oldR & 0x01;
            if (lsbR == aBit)
                return oldR;
            return (byte) (oldR ^ 0x01);
        }

        private static List<byte> MakeNewReds(List<byte> colorReds, List<byte> bitData)
        {
            Debug.Assert(colorReds.Count == bitData.Count);

            List<byte> newColorReds = new List<byte>(colorReds.Count);
            for (int i = 0; i < colorReds.Count; i++)
            {
                byte newR = MakeNewRed(colorReds[i], bitData[i]);
                newColorReds.Add(newR);
            }

            return newColorReds;
        }


        public static void SetImageColorReds(ref Bitmap image, int x, int y, List<byte> newColorReds)
        {
            int idx = 0;
            for (; x < image.Width; x++)
            {
                for (; y < image.Height; y++)
                {
                    if (idx > newColorReds.Count - 1)
                    {
                        return;
                    }

                    Color currentPixel = image.GetPixel(x, y);
                    Color newPixel = Color.FromArgb(newColorReds[idx], currentPixel.G, currentPixel.B);
                    image.SetPixel(x, y, newPixel);

                    idx++;
                }
                y = 0;
            }
        }

        #endregion
    }
}