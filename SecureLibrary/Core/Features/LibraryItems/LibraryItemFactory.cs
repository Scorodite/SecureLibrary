namespace SecureLibrary.Core.Features.LibraryItems
{
    /// <summary>
    /// Base LibraryItem factory class that creates LibraryItem instances based on user input.
    /// If registered, will appear in New Item context menu
    /// </summary>
    public abstract class LibraryItemFactory : Feature
    {
        /// <summary>
        /// Returns instances based on user input. If returned null, item will not be added.
        /// </summary>
        public abstract LibraryItem? Create();
    }
}
