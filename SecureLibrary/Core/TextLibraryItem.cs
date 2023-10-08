using Material.Icons;
using SecureLibrary.Controls;
using SecureLibrary.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SecureLibrary.Core.Custom
{
    public class TextLibraryItem : LibraryItem
    {
        private readonly Lazy<TextBox> _UI;

        private string _Text;

        public TextLibraryItem()
        {
            _UI = new(CreateUI);
            _Text = string.Empty;
            Icon = MaterialIconKind.File;

            Actions.Add(new("To rich", MaterialIconKind.FileText, ToRich_Clicked));
        }

        public TextLibraryItem(string name) : this()
        {
            Name = name;
        }

        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public override TextBox UI => _UI.Value;

        public override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            writer.Write(Text);
        }

        public override void ReadData(BinaryReader reader)
        {
            base.ReadData(reader);
            Text = reader.ReadString();
        }

        private TextBox CreateUI()
        {
            ExtendedTextBox box = new()
            {
                AcceptsReturn = true,
                FontFamily = new("Consolas"),
            };
            box.SetBinding(TextBox.TextProperty, new Binding(nameof(Text)) { Source = this });
            return box;
        }

        private void ToRich_Clicked(MenuButton sender, MainWindow? window)
        {
            LibraryItem owner = Owner!;
            if (RemoveFromOwner())
            {
                RichTextLibraryItem item = new()
                {
                    Name = Name,
                    Icon = Icon == MaterialIconKind.File ? MaterialIconKind.FileText : Icon,
                    IconColor = IconColor,
                    Text = Text,
                };
                owner.Add(item);
                if (window is not null)
                {
                    window.SelectItem(item);
                    if (window.CloseTab(this))
                    {
                        window.OpenTab(item);
                    }
                }
            }
        }
    }
}
