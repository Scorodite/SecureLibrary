using Material.Icons;
using Microsoft.Win32;
using SecureLibrary.Core.Storage;
using System.IO;

namespace SecureLibrary.Core.Features.Storage
{
    [RegisterFeature]
    public class FileStorageItemFactory : BinaryStorageFactory
    {
        public override MaterialIconKind Icon => MaterialIconKind.File;
        public override string Name => "File";

        public override BinaryStorage? Create(StorageFactoryContext context)
        {
            FileDialog dialog = context == StorageFactoryContext.Load ? new OpenFileDialog() : new SaveFileDialog();
            dialog.Title = Name;
            return dialog.ShowDialog() == true ? new FileStorage(dialog.FileName) : null;
        }

        public override BinaryStorage? CreateFromArgs(string[] args)
        {
            if (args.Length == 2 && args[0] == "file" && File.Exists(args[1]))
            {
                return new FileStorage(args[1]);
            }
            else
            {
                return null;
            }
        }
    }
}
