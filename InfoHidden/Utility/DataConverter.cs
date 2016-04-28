using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace InfoHidden.Utility
{

    
    public static class DataConverter
    {

        #region Bits2Byte

        // 0000 0100   0000 1000 ->  4 8
        public static byte Bits2Byte(List<byte> bits)
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
        public static List<byte> Byte2Bits(byte aByte)
        {
            List<byte> bits = new List<byte>(8) { 0, 0, 0, 0, 0, 0, 0, 0, };

            bits[0] = (byte)((aByte & 0x01) >> 0);
            bits[1] = (byte)((aByte & 0x02) >> 1);
            bits[2] = (byte)((aByte & 0x04) >> 2);
            bits[3] = (byte)((aByte & 0x08) >> 3);
            bits[4] = (byte)((aByte & 0x10) >> 4);
            bits[5] = (byte)((aByte & 0x20) >> 5);
            bits[6] = (byte)((aByte & 0x40) >> 6);
            bits[7] = (byte)((aByte & 0x80) >> 7);

            return bits;
        }

        #endregion


        #region Bits2Bytes

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

        #endregion


        #region Bits2Integer

        // 4 ->   0000 0000   0000 0000   0000 0000   0000 0100

        public static int Bits2Integer(List<byte> bits)
        {
            int ret = 0;
            for (int i = 0; i < 32; i++)
            {
                ret |= bits[i] << i;
            }
            return ret;
        }

        public static List<byte> Integer2Bits(int anInteger)
        {
            List<byte> ret = new List<byte>(32);

            for (int i = 0; i < 32; i++)
            {
                ret.Add((byte)GetOneBitFromInteger(anInteger, i));
            }

            return ret;
        }



        #endregion


        #region Bytes2UInt

        public static uint Bytes2UInt(byte[] b)
        {
            var output = (uint)b[3];
            output |= (uint)(b[2] << 8);
            output |= (uint)(b[1] << 16);
            output |= (uint)(b[0] << 24);
            return output;
        }

        public static byte[] UInt2Bytes(uint v)
        {
            byte[] result = new byte[4];
            result[3] = (byte)(v & 0xFF);
            result[2] = (byte)((v >> 8) & 0xFF);
            result[1] = (byte)((v >> 16) & 0xFF);
            result[0] = (byte)((v >> 24) & 0xFF);
            return result;
        }

        #endregion


        #region UInts2Bytes

        public static byte[] UInts2Bytes(uint[] uints)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < uints.Length; i++)
            {
                bytes.AddRange(UInt2Bytes(uints[i]));
            }

            return bytes.ToArray();
        }

        #endregion


        #region Bytes2String

        public static string Bytes2String(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length);

            foreach (byte t in bytes)
            {
                sb.Append((char)t);
            }

            return sb.ToString();
        }

        public static byte[] String2Bytes(string s)
        {
            byte[] ret = new byte[s.Length];

            for (int i = 0; i < s.Length; i++)
            {
                ret[i] = (byte)s[i];
            }

            return ret;
        }

        #endregion

        
        #region GetOneBit From --- Bytes Integer Byte

        private static int GetOneBitFromBytes(byte[] bytesData, int index)
        {
            if ((bytesData[index / 8] & (1 << (index % 8))) == 0)
                return 0;
            else
                return 1;
        }

        public static int GetOneBitFromInteger(int anInteger, int index)
        {
            if ((anInteger & (1 << index)) == 0)
                return 0;
            else
                return 1;
                
        }

        public static int GetOneBitFromByte(byte aByte, int index)
        {
            return aByte & (1 << index);
        }

        #endregion


    }
}