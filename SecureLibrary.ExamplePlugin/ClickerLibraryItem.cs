using Material.Icons;
using SecureLibrary.Core;
using SecureLibrary.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SecureLibrary.ExamplePlugin
{
    public class ClickerLibraryItem : LibraryItem
    {
        private readonly Lazy<Button> _UI;
        private int _ClickCount;

        public ClickerLibraryItem()
        {
            _UI = new(CreateUI);
            Icon = MaterialIconKind.CursorDefaultClick;

            Actions.Add(new("Reset clicks", MaterialIconKind.Restart, ResetClicks_Clicked));
        }

        public ClickerLibraryItem(string name) : this()
        {
            Name = name;
        }

        public override Button UI => _UI.Value;

        public int ClickCount
        {
            get => _ClickCount;
            set
            {
                _ClickCount = value;
                OnPropertyChanged(nameof(ClickCount));
            }
        }

        public override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            writer.Write(ClickCount);
        }

        public override void ReadData(BinaryReader reader)
        {
            base.ReadData(reader);
            ClickCount = reader.ReadInt32();
        }

        private Button CreateUI()
        {
            Button button = new()
            {
                Focusable = false,
            };
            button.SetBinding(
                ContentControl.ContentProperty, 
                new Binding(nameof(ClickCount)) {
                    Source = this,
                }
            );
            button.Click += (s, e) =>
            {
                ClickCount++;
                App.Current.MainWindow.Library?.Resources
                .GetOrNew<TotalClickResource>(TotalClickResource.ResourceName).Increase();
            };
            return button;
        }

        private void ResetClicks_Clicked(MenuButton sender, MainWindow? window)
        {
            ClickCount = 0;
        }
    }
}
