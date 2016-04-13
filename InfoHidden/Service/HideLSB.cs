using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using InfoHidden.Service.ServiceException;
using System.Diagnostics;

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
                    byte randomRed = makeNewRed(currentPixel.R, (new Random()).Next(2));
                    Color randomColor = Color.FromArgb(randomRed, currentPixel.G, currentPixel.B);
                    image.SetPixel(x, y, randomColor);
                }
            }

            return;
        }

        public static void Hide(ref Bitmap image, byte[] data)
        {
            
            int imageBitDataCapacity = image.Width * image.Height;
            int dataBitLength = data.Length * 8;
            
            if ( dataBitLength + FILE_HEAD_BIT_LENGTH > imageBitDataCapacity )
            {
                throw new ImageHideCapacityTooSmallException();
            }

            int? x = null;
            int? y = null;

            writeFileHead(ref image, dataBitLength, out x,  out y);
            
            writeFileBody(ref image, data, (int)x, (int)y);

            return;
        }

        public static byte[] DeHide(Bitmap image)
        {
            int? x = null;
            int? y = null;

            int dataBitLength = readFileHead(image, out x, out y);

            Debug.Assert(dataBitLength % 8 == 0);

            byte[] data = readFileBody(image, dataBitLength, (int)x, (int)y);

            return data;
        }

        #endregion


        #region private methods

        public static void writeFileHead(ref Bitmap image, int dataBitLength, out int? x, out int? y)
        {
            x = 0;
            y = 0;
            List<byte> bitsData = Integer2Bits(dataBitLength);
            byte[] data = Bits2Bytes(bitsData).ToArray();
            writeFileBody(ref image, data, (int)x, (int)y);
            x = 0;
            y = 32;
            return;
        }

        public static int readFileHead(Bitmap image, out int? x, out int? y)
        {
            x = 0;
            y = 0;
            List<byte> retBits = new List<byte>(readFileBody(image, 32, (int)x, (int)y));
            int ret = Bits2Integer(Bytes2Bits(retBits));
            x = 0;
            y = 32;
            return ret;
        }

        public static void writeFileBody(ref Bitmap image, byte[] data,  int x,  int y)
        {

            List<byte> originColorReds = extractColorReds(image, data.Length * 8, x, y);

            List<byte> bitsData = Bytes2Bits(new List<byte>(data));
            int dataBitLength = data.Length * 8;
            List<byte> colorReds = extractColorReds(image, dataBitLength, x, y);
            List<byte> newColorReds = makeNewReds(colorReds, bitsData);
            setImageColorReds(ref image, x, y, newColorReds);

            return;
        }

        public static byte[] readFileBody(Bitmap image, int dataBitLength, int x, int y)
        {
            try
            {
                List<byte> colorReds = extractColorReds(image, dataBitLength, x, y);
                List<byte> bits = extractBitsFromColorRedsLsb(colorReds);
                List<byte> bytes = Bits2Bytes(bits);

                return bytes.ToArray();
            }
            catch (Exception)
            {

                throw new PasswordWrongException();
            }
        }


        // 1100 0000 1000 0000 ->  3 1
        public static List<byte> Bits2Bytes(List<byte> bits)
        {
            List<byte> retBytes = new List<byte>(bits.Count / 8);

            List<byte> bitsForOneByte = new List<byte>(8) { 0, 0, 0, 0, 0, 0, 0, 0, };
            for (int i = 0; i < bits.Count; i += 8)
            {

                bitsForOneByte[0] = bits[i + 0];
                bitsForOneByte[1] = bits[i + 1];
                bitsForOneByte[2] = bits[i + 2];
                bitsForOneByte[3] = bits[i + 3];
                bitsForOneByte[4] = bits[i + 4];
                bitsForOneByte[5] = bits[i + 5];
                bitsForOneByte[6] = bits[i + 6];
                bitsForOneByte[7] = bits[i + 7];

                retBytes.Add(Bits2Byte(bitsForOneByte));
            }

            Debug.Assert(retBytes.Count == bits.Count / 8);

            return retBytes;
        }

        //  3 1   ->   1100 0000 1000 0000 
        public static List<byte> Bytes2Bits(List<byte> bytes)
        {
            int bitsLength = bytes.Count * 8;
            List<byte> retBits = new List<byte>();

            for (int i = 0; i < bytes.Count; i++)
            {
                retBits.AddRange(Byte2Bits(bytes[i]));
            }

            return retBits;
        }



        // 0000 0100   0000 1000 ->  4 8
        private static byte Bits2Byte(List<byte> bits)
        {
            int ret = bits[0];
            ret |= bits[1] << 1;
            ret |= bits[2] << 2;
            ret |= bits[3] << 3;
            ret |= bits[4] << 4;
            ret |= bits[5] << 5;
            ret |= bits[6] << 6;
            ret |= bits[7] << 7;

            return (byte)ret;
        }

        // 4 8   ->  0000 0100   0000 1000 
        private static List<byte> Byte2Bits(byte aByte)
        {
            List<byte> bits = new List<byte>(8) {0, 0, 0, 0,    0, 0, 0, 0, };

            bits[0] = (byte) ((aByte & 0x01) >> 0);
            bits[1] = (byte) ((aByte & 0x02) >> 1);
            bits[2] = (byte) ((aByte & 0x04) >> 2);
            bits[3] = (byte) ((aByte & 0x08) >> 3);
            bits[4] = (byte) ((aByte & 0x10) >> 4);
            bits[5] = (byte) ((aByte & 0x20) >> 5);
            bits[6] = (byte) ((aByte & 0x40) >> 6);
            bits[7] = (byte) ((aByte & 0x80) >> 7);

            return bits;
        }

        // 4 ->   0000 0000   0000 0000   0000 0000   0000 0100
        public static List<byte> Integer2Bits(int anInteger)
        {
            List<byte> ret = new List<byte>(32);

            for (int i = 0; i < 32; i++)
            {
                ret.Add((byte)getOneBitFromInteger(anInteger, i));
            }

            return ret;
        }

        public static int Bits2Integer(List<byte> bits)
        {
            int ret = 0;
            for (int i = 0; i < 32; i++)
            {
                ret |= bits[i] << i;
            }
            return ret;
        }

        private static byte readLsbBitFromColorR(Color color)
        {
            return (byte)(color.R & 0x01);
        }

        private static int getOneBitFromBytes(byte[] bytesData, int index)
        {
            if ((bytesData[index / 8] & (1 << (index % 8))) == 0)
                return 0;
            else
                return 1;
        }

        private static int getOneBitFromInteger(int anInteger, int index)
        {
            if ((anInteger & (1 << index)) == 0)
                return 0;
            else
                return 1;
                
        }

        private static int getOneBitFromByte(byte aByte, int index)
        {
            return aByte & (1 << index);
        }

        public static List<byte> extractColorReds(Bitmap image, int length, int x, int y)
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

        public static List<byte> extractBitsFromColorRedsLsb(List<byte> colorReds)
        {
            List<byte> bits = new List<byte>(colorReds.Count);
            for (int i = 0; i < colorReds.Count; i++)
            {
                bits.Add((byte)getOneBitFromByte(colorReds[i], 0));
            }

            return bits;
        }



        private static byte makeNewRed(byte oldR, int aBit)
        {
            int lsbR = oldR & 0x01;
            if (lsbR == aBit)
                return oldR;
            else
                return (byte)(oldR ^ 0x01);
        }

        private static List<byte> makeNewReds(List<byte> colorReds, List<byte> bitData)
        {
            Debug.Assert(colorReds.Count == bitData.Count);

            List<byte> newColorReds = new List<byte>(colorReds.Count);
            for (int i = 0; i < colorReds.Count; i++)
            {
                byte newR = makeNewRed(colorReds[i], bitData[i]);
                newColorReds.Add(newR);
            }

            return newColorReds;
        }



        public static void setImageColorReds(ref Bitmap image, int x, int y, List<byte> newColorReds)
        {
            int idx = 0;
            for (; x < image.Width; x++)
            {
                for (; y < image.Height; y++)
                {
                    if ( idx > newColorReds.Count-1)
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
