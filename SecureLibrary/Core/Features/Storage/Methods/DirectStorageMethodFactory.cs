using Material.Icons;
using SecureLibrary.Core.Storage.Methods;

namespace SecureLibrary.Core.Features.Storage.Methods
{
    [RegisterFeature]
    public class DirectStorageMethodFactory : StorageMethodFactory
    {
        public override string Name => "Direct";
        public override MaterialIconKind Icon => MaterialIconKind.Text;

        public override StorageMethod? Create(StorageFactoryContext context)
        {
            return DirectStorageMethod.Instance;
        }
    }
}
