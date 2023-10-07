using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Core.Storage
{
    /// <summary>
    /// Storage that contains library binary data
    /// </summary>
    public abstract class BinaryStorage
    {
        public abstract string Name { get; }

        public abstract Stream OpenWrite();
        public abstract Stream OpenRead();
    }
}
