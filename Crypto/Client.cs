using System;
using System.CodeDom.Compiler;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using Benalo.classes;
using Benalo.interfaces;
using filemanager;
using Magenta;

namespace Crypto
{
    class Client
    {
        private TcpClient? client = new TcpClient();

        private string host = string.Empty;
        private int port = 0;
        private byte[] sessionKey;
        private bool connectionStatus = false;

        public FileEncryptor encryptor = new FileEncryptor();
        public FileDecryptor decryptor = new FileDecryptor();

        public void connect(string host, int port)
        {
            if (connectionStatus)
                return;

            if (client == null)
                client = new TcpClient();

            if (client.Connected)
            {
                disconnect();
                client = new TcpClient(host, port);
            }
            else
            {
                try
                {
                    client.Connect(host, port);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            connectionStatus = true;

            if (!client.Connected)
                return;

            var stream = client.GetStream();
            var binaryReader = new BinaryReader(stream);

            string str = binaryReader.ReadString();
            while (str != "key")
            { }

            Key key = new Key();
            int size = binaryReader.ReadInt32();
            key.y = new BigInteger(binaryReader.ReadBytes(size));
            size = binaryReader.ReadInt32();
            key.r = new BigInteger(binaryReader.ReadBytes(size));
            size = binaryReader.ReadInt32();
            key.n = new BigInteger(binaryReader.ReadBytes(size));

            var benalohClass = new AsymmetricEncryptionDecryption(key);

            var rnd = new Random();

            var bytes = new Byte[16];

            rnd.NextBytes(bytes);

            var binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write("sessionKey");

            for (int i = 0; i < bytes.Length; i += 2)
            {
                var temp = bytes.ToList().GetRange(i, 2);
                var _sessionKey = new BigInteger(temp.ToArray(), true);

                var encryptedKey = benalohClass.Encryption(_sessionKey);
                size = encryptedKey.GetByteCount();
                binaryWriter.Write(size);
                binaryWriter.Write(encryptedKey.ToByteArray());
            }

            var str_ = binaryReader.ReadString();

            binaryWriter.Flush();

            sessionKey = bytes;

            connectionStatus = false;
        }

        public bool reconnect()
        {
            if (host == string.Empty || port == 0)
            {
                return false;
            }
            else
            {
                connect(host, port);
                return true;
            }
        }

        public bool isConnected()
        {
            if (client != null)
                return client.Connected;
            else
                return false;
        }

        public void disconnect()
        {
            client.Close();
            client = null;
        }

        public bool sendFile(string filePath)
        {
            if (encryptor.process != 0 && encryptor.process != 100) 
            {
                return false;
            }

            if (client.Connected)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    encryptor.Encrypt(filePath, "encrypted", sessionKey);
                    FileInfo encryptedInfo = new FileInfo("encrypted");
                    var size = encryptedInfo.Length;

                    var stream = client.GetStream();
                    var binaryWriter = new BinaryWriter(stream);

                    binaryWriter.Write("file");
                    binaryWriter.Write(fileInfo.Name);
                    binaryWriter.Write(size);
                    binaryWriter.Write(encryptor.paddingCount);

                    FileStream f_in = new FileStream("encrypted", FileMode.Open);
                    for (int i = 0; i < size / 16; ++i)
                    {
                        byte[] bytes = new byte[16];
                        f_in.Read(bytes, 0, bytes.Length);
                        binaryWriter.Write(bytes);
                    }
                    f_in.Close();

                    File.Delete("encrypted");

                    var binaryReader = new BinaryReader(stream);
                    //binaryWriter.Flush();
                    if (binaryReader.ReadString() == "OK")
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public bool getFile(string fileName)
        {
            if (fileName == null)
                return false;

            if (client != null && client.Connected)
            {
                var stream = client.GetStream();
                var binaryWriter = new BinaryWriter(stream);
                var binaryReader = new BinaryReader(stream);

                binaryWriter.Write("down");
                binaryWriter.Write(fileName);

                string name = binaryReader.ReadString();

                if (name == "Dont")
                    return false;

                int size = (int)binaryReader.ReadInt64();
                int padding = binaryReader.ReadInt32();

                File.Delete("encrypted");
                File.Delete(name);
                FileStream f_in = new FileStream("encrypted", FileMode.Create);

                for (int i = 0; i < size / 16; ++i)
                {
                    byte[] bytes = binaryReader.ReadBytes(16);
                    f_in.Write(bytes);
                }
                f_in.Close();

                decryptor.Decrypt("encrypted", fileName, sessionKey, padding);
                File.Delete("encrypted");
                binaryWriter.Write("OK");

                Console.WriteLine($"File {name} downloaded to server");

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
