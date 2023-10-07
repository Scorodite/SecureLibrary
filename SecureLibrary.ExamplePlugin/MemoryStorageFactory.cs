using Material.Icons;
using SecureLibrary.Core.Features;
using SecureLibrary.Core.Features.Storage;
using SecureLibrary.Core.Storage;

namespace SecureLibrary.ExamplePlugin
{
    [RegisterFeature]
    public class MemoryStorageFactory : BinaryStorageFactory
    {
        public override string Name => "Memory";
        public override MaterialIconKind Icon => MaterialIconKind.Memory;

        public override BinaryStorage? Create(StorageFactoryContext context)
        {
            if (context != StorageFactoryContext.Load ||
                MemoryStorage.Storage.Length > 0)
            {
                return MemoryStorage.Instance;
            }
            else
            {
                App.MessageBox("Error", "Memory storage is empty");
                return null;
            }
        }
    }
}