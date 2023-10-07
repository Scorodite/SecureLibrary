using Material.Icons;
using SecureLibrary.Core.Custom;

namespace SecureLibrary.Core.Features.LibraryItems
{
    [RegisterFeature]
    public class RichTextLibraryItemFactory : LibraryItemFactory
    {
        public override MaterialIconKind Icon => MaterialIconKind.FileText;
        public override string Name => "Rich textual";

        public override LibraryItem? Create()
        {
            return App.InputBox("Enter name") is string name ? new RichTextLibraryItem(name) : null;
        }
    }
}
