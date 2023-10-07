using Material.Icons;
using Microsoft.Win32;
using SecureLibrary.Core.Storage;
using System.IO;
using System.Windows;

namespace SecureLibrary.Core.Features.Storage
{
    [RegisterFeature]
    public class AttachedStorageFactory : BinaryStorageFactory
    {
        public override MaterialIconKind Icon => MaterialIconKind.Attachment;
        public override string Name => "Attached";

        public override BinaryStorage? Create(StorageFactoryContext context)
        {
            OpenFileDialog dialog = new()
            {
                Title = Name
            };
            return dialog.ShowDialog() == true ?
                       new AttachedFileStorage(dialog.FileName) :
                       null;
        }

        public override BinaryStorage? CreateFromArgs(string[] args)
        {
            if (args.Length == 2 && args[0] == "attached" && File.Exists(args[1]))
            {
                return new AttachedFileStorage(args[1]);
            }
            else
            {
                return null;
            }
        }
    }
}
