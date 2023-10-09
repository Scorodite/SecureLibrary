using Material.Icons;
using SecureLibrary.Core.Storage;
using SecureLibrary.Core.Storage.Methods;
using System.Windows;

namespace SecureLibrary.Core.Features.Storage.Methods
{
    [RegisterFeature]
    public class AesStorageMethodFactory : StorageMethodFactory
    {
        public override string Name => "Encrypted (AES)";
        public override MaterialIconKind Icon => MaterialIconKind.Lock;

        public override StorageMethod? Create(StorageFactoryContext context)
        {
            if (App.PasswordBox("Enter password") is string password)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    App.MessageBox("Error", "Password can not be empty",
                                   MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                else
                {
                    return new AesStorageMethod(password);
                }
            }
            return null;
        }

        public override StorageMethod? CreateFromArgs(string[] args)
        {
            if (args.Length == 1 || args.Length == 2)
            {
                string? password = args.Length == 2 ? args[1] : App.PasswordBox("Enter password");
                if (string.IsNullOrWhiteSpace(password))
                {
                    App.MessageBox("Error", "Password can not be empty",
                                   MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
                else
                {
                    return new AesStorageMethod(password);
                }
            }
            return null;
        }
    }
}
