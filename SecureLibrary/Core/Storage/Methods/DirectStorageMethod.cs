using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SecureLibrary.Core.Storage.Methods
{
    public class DirectStorageMethod : StorageMethod
    {
        public static DirectStorageMethod Instance { get; }

        static DirectStorageMethod()
        {
            Instance = new();
        }

        protected DirectStorageMethod() { }

        public override string Name => "Direct";

        public override Stream OpenRead(BinaryStorage storage)
        {
            return storage.OpenRead();
        }

        public override Stream OpenWrite(BinaryStorage storage)
        {
            return storage.OpenWrite();
        }
    }
}
