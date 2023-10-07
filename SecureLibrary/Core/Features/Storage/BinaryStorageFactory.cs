using SecureLibrary.Core.Storage;

namespace SecureLibrary.Core.Features.Storage
{
    /// <summary>
    /// Base BinaryStorage factory class that creates BinaryStorage instances based on user input
    /// If registered, will appear in New, Open and Save context menus
    /// </summary>
    public abstract class BinaryStorageFactory : Feature
    {
        public abstract BinaryStorage? Create(StorageFactoryContext context);

        public virtual BinaryStorage? CreateFromArgs(string[] args)
        {
            return null;
        }
    }
}
