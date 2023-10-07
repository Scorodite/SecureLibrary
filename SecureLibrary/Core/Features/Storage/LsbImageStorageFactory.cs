using Material.Icons;
using Microsoft.Win32;
using SecureLibrary.Core.Storage;
using System.IO;

namespace SecureLibrary.Core.Features.Storage
{
    [RegisterFeature]
    public class LsbImageStorageFactory : BinaryStorageFactory
    {
        public override MaterialIconKind Icon => MaterialIconKind.Image;
        public override string Name => "LSB Image";

        public override BinaryStorage? Create(StorageFactoryContext context)
        {
            OpenFileDialog dialog = new()
            {
                Title = Name,
                Filter = "Pixel Image|*.png;*.bmp",
            };
            return dialog.ShowDialog() == true ? new LsbImageStorage(dialog.FileName) : null;
        }

        public override BinaryStorage? CreateFromArgs(string[] args)
        {
            if (args.Length == 2 && args[0] == "lsb" && File.Exists(args[1]))
            {
                return new LsbImageStorage(args[1]);
            }
            else
            {
                return null;
            }
        }
    }
}
