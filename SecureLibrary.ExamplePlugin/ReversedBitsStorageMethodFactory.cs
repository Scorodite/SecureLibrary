using Material.Icons;
using SecureLibrary.Core.Features;
using SecureLibrary.Core.Features.Storage;
using SecureLibrary.Core.Features.Storage.Modifiers;
using SecureLibrary.Core.Storage.Methods;

namespace SecureLibrary.ExamplePlugin
{
    [RegisterFeature]
    public class ReversedBitsStorageMethodFactory : StorageMethodFactory
    {
        public override string Name => "Reversed bits";
        public override MaterialIconKind Icon => MaterialIconKind.FunctionVariant;

        public override StorageMethod? Create(StorageFactoryContext context)
        {
            return ReversedBitsStorageMethod.Instance;
        }
    }
}