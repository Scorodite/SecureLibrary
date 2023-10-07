using Material.Icons;
using SecureLibrary.Core.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SecureLibrary.Core.Features.LibraryItems
{
    [RegisterFeature]
    public class TextLibraryItemFactory : LibraryItemFactory
    {
        public override MaterialIconKind Icon => MaterialIconKind.File;
        public override string Name => "Textual";

        public override LibraryItem? Create()
        {
            return App.InputBox("Enter name") is string name ? new TextLibraryItem(name) : null;
        }
    }
}
