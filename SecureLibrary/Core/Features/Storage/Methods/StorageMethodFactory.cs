using SecureLibrary.Core.Storage;
using SecureLibrary.Core.Storage.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Core.Features.Storage.Modifiers
{
    /// <summary>
    /// Base StorageMethod factory class that creates StorageMethod instances based on user input.
    /// If registered, will appear in New, Open and Save context menus
    /// </summary>
    public abstract class StorageMethodFactory : Feature
    {
        public abstract StorageMethod? Create(StorageFactoryContext context);

        public virtual StorageMethod? CreateFromArgs(string[] args)
        {
            return null;
        }
    }
}
