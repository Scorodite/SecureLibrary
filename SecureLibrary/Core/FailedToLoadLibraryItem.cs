using Material.Icons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Core
{
    /// <summary>
    /// LibraryItem in which item`s data is stored if it fails to load
    /// </summary>
    public class FailedToLoadLibraryItem : LibraryItem
    {
        public FailedToLoadLibraryItem(string type, Stream data)
        {
            Name = "Failed to load";
            Icon = MaterialIconKind.Alert;

            Type = type;
            Data = data;
        }

        public string Type { get; set; }
        public Stream Data { get; set; }

        public override void ReadData(BinaryReader reader)
        {
            MemoryStream data = new((int)reader.BaseStream.Length);
            reader.BaseStream.CopyTo(data);
            data.Position = 0;
            Data = data;
        }

        public override void WriteData(BinaryWriter writer)
        {
            Data.Position = 0;
            Data.CopyTo(writer.BaseStream);
        }
    }
}
