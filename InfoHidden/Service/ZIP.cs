﻿namespace InfoHidden.Service
{
    using System;
    using System.IO;
    using System.IO.Compression;

    using InfoHidden.Service.ServiceException;

    public class Zip
    {
        #region Public methods

        public static byte[] Compress(byte[] fileBytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Compress))
                {
                    gzipStream.Write(fileBytes, 0, fileBytes.Length);
                }

                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] fileBytes)
        {
            try
            {
                using (MemoryStream srcMs = new MemoryStream(fileBytes))
                {
                    using (GZipStream zipStream = new GZipStream(srcMs, CompressionMode.Decompress))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] bytes = new byte[40960];
                            int n;
                            while ((n = zipStream.Read(bytes, 0, bytes.Length)) > 0)
                            {
                                ms.Write(bytes, 0, n);
                            }

                            zipStream.Close();

                            return ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new PasswordWrongException();
            }
        }

        #endregion
    }
}