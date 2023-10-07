using Material.Icons;

namespace SecureLibrary.Core.Features.LibraryItems
{
    [RegisterFeature]
    public class EmptyLibraryItemFactory : LibraryItemFactory
    {
        public override MaterialIconKind Icon => MaterialIconKind.Folder;
        public override string Name => "Empty";
        public override float Priority => 1;

        public override LibraryItem? Create()
        {
            return App.InputBox("Enter name") is string name ? new(name) : null;
        }
    }
}
