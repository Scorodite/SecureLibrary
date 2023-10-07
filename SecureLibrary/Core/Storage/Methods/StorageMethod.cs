using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Core.Storage.Methods
{
    /// <summary>
    /// Method that determines how raw library data will be modified
    /// before writing in BinaryStorage (encrypted, obfuscated, etc.)
    /// </summary>
    public abstract class StorageMethod
    {
        public abstract string Name { get; }

        public abstract Stream OpenWrite(BinaryStorage storage);
        public abstract Stream OpenRead(BinaryStorage storage);
    }
}
