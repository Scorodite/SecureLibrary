using Material.Icons;
using SecureLibrary.Core;
using SecureLibrary.Core.Features;
using SecureLibrary.Core.Features.LibraryItems;
using SecureLibrary.Windows;

namespace SecureLibrary.ExamplePlugin
{
    [RegisterFeature]
    public class ClickerLibraryItemFactory : LibraryItemFactory
    {

        public override string Name => "Clicker";
        public override MaterialIconKind Icon => MaterialIconKind.CursorDefaultClick;

        public override LibraryItem? Create()
        {
            return
                App.InputBox("Enter name") is string name ?
                    new ClickerLibraryItem(name) :
                    null;
        }

        public override void Init(App app)
        {
            app.MainWindow.MenuButtons.Add(
                new("Total clicks", MaterialIconKind.CursorDefaultClick, TotalClicks_Clicked)
            );
        }

        private void TotalClicks_Clicked(MenuButton sender, MainWindow? window)
        {
            if (window?.Library is Library lib)
            {
                App.MessageBox(
                    "Total clicks",
                    "Total clicks: " +
                    lib.Resources.GetOrNew<TotalClickResource>(TotalClickResource.ResourceName)
                        .TotalClicks
                );
            }
            else
            {
                App.MessageBox("Total clicks", "Library is not open");
            }
        }
    }
}