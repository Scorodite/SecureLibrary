using SecureLibrary.Utilities.Streams;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecureLibrary.Core.Storage.Methods
{
    public class AesStorageMethod : StorageMethod
    {
        public AesStorageMethod(byte[] key)
        {
            Key = key;
        }

        public AesStorageMethod(string password) : this(SHA256.HashData(Encoding.UTF8.GetBytes(password))) { }

        public override string Name => "AES";

        public byte[] Key { get; }

        public override Stream OpenRead(BinaryStorage storage)
        {
            return new AesStream(storage.OpenRead(), Key, true, CryptoStreamMode.Read);
        }

        public override Stream OpenWrite(BinaryStorage storage)
        {
            return new AesStream(storage.OpenWrite(), Key, false, CryptoStreamMode.Write);
        }
    }
}
