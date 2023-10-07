using Material.Icons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Input;
using System.Windows.Documents;
using SecureLibrary.Controls;
using SecureLibrary.Windows;

namespace SecureLibrary.Core.Custom
{
    public class RichTextLibraryItem : LibraryItem
    {
        private readonly Lazy<RichTextEditor> _UI;
        private MemoryStream? UnloadedDataStream;

        public RichTextLibraryItem()
        {
            _UI = new(CreateUI);
            Icon = MaterialIconKind.FileText;
            SortWeight = 1;

            Actions.Add(new("To not rich", MaterialIconKind.File, ToNotRich_Clicked));
        }

        public RichTextLibraryItem(string name) : this()
        {
            Name = name;
        }

        ~RichTextLibraryItem()
        {
            UnloadedDataStream?.Dispose();
        }

        public string Text
        {
            get => new TextRange(UI.Document.ContentStart, UI.Document.ContentEnd).Text;
            set => new TextRange(UI.Document.ContentStart, UI.Document.ContentEnd).Text = value;
        }

        public override RichTextEditor UI => _UI.Value;

        private bool IsUiLoaded => _UI.IsValueCreated;

        private RichTextEditor CreateUI()
        {
            RichTextEditor box = new() { };
            if (UnloadedDataStream is not null &&
                UnloadedDataStream.Length > 0)
            {
                LoadTextData(box, UnloadedDataStream);
                UnloadedDataStream.Dispose();
                UnloadedDataStream = null;
            }
            // Required to avoid triggering OnPropertyChanged if text data has been loaded
            box.Dispatcher.InvokeAsync(async () => {
                await Task.Delay(100);
                box.TextChanged += (s, e) => OnPropertyChanged(nameof(Text));
            });
            return box;
        }

        public override void WriteData(BinaryWriter writer)
        {
            base.WriteData(writer);
            if (IsUiLoaded)
            {
                SaveTextData(UI, writer.BaseStream);
            }
            else
            {
                UnloadedDataStream?.CopyTo(writer.BaseStream);
            }
        }

        public override void ReadData(BinaryReader reader)
        {
            base.ReadData(reader);
            UnloadedDataStream = new();
            reader.BaseStream.CopyTo(UnloadedDataStream);
            UnloadedDataStream.Position = 0;
        }

        private static void SaveTextData(RichTextBox box, Stream stream)
        {
            TextRange range = new(box.Document.ContentStart, box.Document.ContentEnd);
            range.Save(stream, DataFormats.Rtf);
        }

        private static void LoadTextData(RichTextBox box, Stream stream)
        {
            TextRange range = new(box.Document.ContentStart, box.Document.ContentEnd);
            range.Load(stream, DataFormats.Rtf);
        }

        private void ToNotRich_Clicked(MenuButton sender, MainWindow? window)
        {
            LibraryItem owner = Owner!;
            if (RemoveFromOwner())
            {
                TextLibraryItem item = new()
                {
                    Name = Name,
                    Icon = Icon == MaterialIconKind.FileText ? MaterialIconKind.File : Icon,
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
