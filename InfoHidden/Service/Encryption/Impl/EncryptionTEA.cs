using System;
using InfoHidden.Service.ServiceException;
using InfoHidden.Utility;

namespace InfoHidden.Service.Encryption.Impl
{
    public class EncryptionTEA : IEncryption
    {
        public byte[] Encrypt(byte[] plaintext, uint[] key)
        {
            byte[] paddedPlainText = PaddingData(plaintext);
            byte[] ciphertext = new byte[paddedPlainText.Length];
            uint[] v = new uint[2];

            byte[] tmp = new byte[4];
            for (int i = 0; i < paddedPlainText.Length; i += 8)
            {
                v[0] =
                    DataConverter.Bytes2UInt(new[]
                    {paddedPlainText[i + 0], paddedPlainText[i + 1], paddedPlainText[i + 2], paddedPlainText[i + 3]});
                v[1] =
                    DataConverter.Bytes2UInt(new[]
                    {paddedPlainText[i + 4], paddedPlainText[i + 5], paddedPlainText[i + 6], paddedPlainText[i + 7]});
                Encipher(v, key);
                tmp = DataConverter.UInt2Bytes(v[0]);
                Array.Copy(tmp, 0, ciphertext, i, 4);
                tmp = DataConverter.UInt2Bytes(v[1]);
                Array.Copy(tmp, 0, ciphertext, i + 4, 4);
            }

            return ciphertext;
        }

        public byte[] Decrypt(byte[] ciphertext, uint[] key)
        {
            byte[] paddedPlaintext = new byte[ciphertext.Length];

            uint[] v = new uint[2];
            byte[] tmp = new byte[4];
            for (int i = 0; i < ciphertext.Length; i += 8)
            {
                v[0] =
                    DataConverter.Bytes2UInt(new[]
                    {ciphertext[i + 0], ciphertext[i + 1], ciphertext[i + 2], ciphertext[i + 3]});
                v[1] =
                    DataConverter.Bytes2UInt(new[]
                    {ciphertext[i + 4], ciphertext[i + 5], ciphertext[i + 6], ciphertext[i + 7]});
                Deciper(v, key);
                tmp = DataConverter.UInt2Bytes(v[0]);
                Array.Copy(tmp, 0, paddedPlaintext, i, 4);
                tmp = DataConverter.UInt2Bytes(v[1]);
                Array.Copy(tmp, 0, paddedPlaintext, i + 4, 4);
            }

            return DepaddingData(paddedPlaintext);
        }


        private static byte[] PaddingData(byte[] originData)
        {
            long originLength = originData.Length;
            int paddingLength = 8 - (int) (originLength%8);

            byte[] paddedData = new byte[originLength + paddingLength];

            paddedData[0] = (byte) paddingLength;
            for (int i = 1; i < paddingLength; i++)
            {
                paddedData[i] = 0;
            }
            Array.Copy(originData, 0, paddedData, paddingLength, originLength);

            return paddedData;
        }

        private static byte[] DepaddingData(byte[] originData)
        {
            try
            {
                int paddingLength = originData[0];
                byte[] depaddedData = new byte[originData.Length - paddingLength];

                Array.Copy(originData, paddingLength, depaddedData, 0, originData.Length - paddingLength);

                return depaddedData;
            }
            catch (Exception)
            {
                throw new PasswordWrongException();
            }
        }

        public static void Encipher(uint[] v, uint[] key)
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
        }

        public static void Deciper(uint[] v, uint[] key)
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
        }
    }
}