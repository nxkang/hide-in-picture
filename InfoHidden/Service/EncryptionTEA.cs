using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using InfoHidden.Service.ServiceException;

namespace InfoHidden.Service
{

    public static class EncryptionTEA 
    {
        public static byte[] Encrypt(byte[] plaintext, uint[] key)
        {
            byte[] paddedPlainText = paddingData(plaintext);
            byte[] ciphertext = new byte[paddedPlainText.Length];
            uint[] v = new uint[2];

            byte[] tmp = new byte[4];
            for (int i = 0; i < paddedPlainText.Length; i += 8)
            {
                v[0] = ByteArray2UInt(new byte[] { paddedPlainText[i + 0], paddedPlainText[i + 1], paddedPlainText[i + 2], paddedPlainText[i + 3] });
                v[1] = ByteArray2UInt(new byte[] { paddedPlainText[i + 4], paddedPlainText[i + 5], paddedPlainText[i + 6], paddedPlainText[i + 7] });
                encipher(v, key);
                tmp = UInt2ByteArray(v[0]);
                Array.Copy(tmp, 0, ciphertext, i, 4);
                tmp = UInt2ByteArray(v[1]);
                Array.Copy(tmp, 0, ciphertext, i+4, 4);
            }

            return ciphertext;
        }

        public static byte[] Decrypt(byte[] ciphertext, uint[] key)
        {

            byte[] paddedPlaintext = new byte[ciphertext.Length];

            uint[] v = new uint[2];
            byte[] tmp = new byte[4];
            for (int i = 0; i < ciphertext.Length; i += 8)
            {
                v[0] = ByteArray2UInt(new byte[] { ciphertext[i + 0], ciphertext[i + 1], ciphertext[i + 2], ciphertext[i + 3] });
                v[1] = ByteArray2UInt(new byte[] { ciphertext[i + 4], ciphertext[i + 5], ciphertext[i + 6], ciphertext[i + 7] });
                deciper(v, key);
                tmp = UInt2ByteArray(v[0]);
                Array.Copy(tmp, 0, paddedPlaintext, i, 4);
                tmp = UInt2ByteArray(v[1]);
                Array.Copy(tmp, 0, paddedPlaintext, i + 4, 4);
            }

            return depaddingData(paddedPlaintext);
            
        }


        private static byte[] paddingData(byte[] originData)
        {
            long originLength = originData.Length;
            int paddingLength = 8 - (int)(originLength % 8);

            byte[] paddedData = new byte[originLength + paddingLength];

            paddedData[0] = (byte)paddingLength;
            for (int i = 1; i < paddingLength; i++)
            {
                paddedData[i] = (byte)0;
            }
            Array.Copy(originData, 0, paddedData, paddingLength, originLength);

            return paddedData;
        }

        private static byte[] depaddingData(byte[] originData)
        {
            try
            {
                int paddingLength = (int)originData[0];
                byte[] depaddedData = new byte[originData.Length - paddingLength];

                Array.Copy(originData, paddingLength, depaddedData, 0, originData.Length - paddingLength);

                return depaddedData;
            }
            catch (Exception)
            {
                throw new PasswordWrongException();
            }
            
        }

        public static void encipher(uint[] v, uint[] key)
        {
            uint v0 = v[0];
            uint v1 = v[1];
            uint delta = 0x9E3779B9;
            uint sum = 0;

            for (int i = 0; i < 32; i++)
            {
                v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
                sum += delta;
                v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
            }
            v[0] = v0;
            v[1] = v1;

            return;
        }

        public static void deciper(uint[] v, uint[] key)
        {
            uint v0 = v[0];
            uint v1 = v[1];
            uint delta = 0x9E3779B9;
            uint sum = delta*32;

            for (int i = 0; i < 32; i++)
            {
                v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
                sum -= delta;
                v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
            }
            v[0] = v0;
            v[1] = v1;

            return;
        }

        public static byte[] UInt2ByteArray(uint v)
        {
            byte[] result = new byte[4];
            result[3] = (byte)(v & 0xFF);
            result[2] = (byte)((v >> 8) & 0xFF);
            result[1] = (byte)((v >> 16) & 0xFF);
            result[0] = (byte)((v >> 24) & 0xFF);
            return result;
        }

        public static uint ByteArray2UInt(byte[] b)
        {
            
            uint output;
            output = (uint)b[3];
            output |= (uint)(b[2] << 8);
            output |= (uint)(b[1] << 16);
            output |= (uint)(b[0] << 24);
            return output;
        }


    }
}
