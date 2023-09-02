using Magenta;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace filemanager
{
    internal class FileEncryptor
    {
        public int process;

        FileStream? f_in;
        FileStream? f_out;
        public int paddingCount = 0;
        public void Encrypt(string filePath, string encryptedPath, byte[] key, byte[]? iniVector = null)
        {
            process = 0;
            var fileInfo = new FileInfo(filePath);

            var blocksCount = fileInfo.Length / 16;
            paddingCount = (int)((16 - fileInfo.Length % 16) % 16);

            f_in = new FileStream(filePath, FileMode.Open);
            f_out = new FileStream(encryptedPath, FileMode.Create);

            byte[] bytes = new byte[16];
            for (int i = 0; i < blocksCount; i++)
            {
                f_in.Read(bytes, 0, 16);

                if (iniVector == null)
                {
                    ECB_EncryptBlock(bytes, key);
                }

                process = (int)(((double) i / blocksCount) * 100);
            }

            bytes = new byte[16];
            f_in.Read(bytes, 0, (int)(16 - paddingCount));
            Array.Fill(bytes, (byte)paddingCount, (int)(16 - paddingCount), (int)paddingCount);

            if (iniVector == null)
            {
                ECB_EncryptBlock(bytes, key);
            }

            process = 100;
            f_in.Close();
            f_out.Close();
        }

        private byte[] ECB_EncryptBlock(byte[] block, byte[] key)
        {
            byte[] res = MagentaCr.Encrypt(block, key);

            f_out.Write(res, 0, res.Length);

            return res;
        }
    }

    internal class FileDecryptor
    {
        FileStream? f_in;
        FileStream? f_out;
        int paddingCount = 0;

        public void Decrypt(string filePath, string decryptedPath, byte[] key, int paddingCount, byte[]? iniVector = null)
        {
            var fileInfo = new FileInfo(filePath);

            var blocksCount = fileInfo.Length / 16;
            this.paddingCount = paddingCount;

            f_in = new FileStream(filePath, FileMode.Open);
            f_out = new FileStream(decryptedPath, FileMode.Create);

            byte[] bytes = new byte[16];

            if (paddingCount > 0) { blocksCount--; }

            for (int i = 0; i < blocksCount; i++)
            {
                f_in.Read(bytes, 0, 16);

                if (iniVector == null)
                {
                    ECB_DecryptBlock(bytes, key, GetF_out());
                }
            }

            if (paddingCount > 0)
            {
                f_in.Read(bytes, 0, 16);
                byte[] res = MagentaCr.Decrypt(bytes, key);

                Array.Fill<byte>(res, 0, 16 - paddingCount, paddingCount);

                f_out.Write(res, 0, res.Length - paddingCount);
            }


            f_in.Close();
            f_out.Close();
        }

        private FileStream? GetF_out()
        {
            return f_out;
        }

        private byte[] ECB_DecryptBlock(byte[] block, byte[] key, FileStream? f_out)
        {
            byte[] res = MagentaCr.Decrypt(block, key);
            f_out.Write(res, 0, res.Length);

            return res;
        }
    }

}
