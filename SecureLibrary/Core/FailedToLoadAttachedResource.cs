using System.IO;

namespace SecureLibrary.Core
{
    /// <summary>
    /// AttachedResource in which item`s data is stored if it fails to load
    /// </summary>
    public class FailedToLoadAttachedResource : AttachedResource
    {
        public FailedToLoadAttachedResource(string type, Stream data)
        {
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
