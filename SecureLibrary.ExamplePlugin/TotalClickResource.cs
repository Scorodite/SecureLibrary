using SecureLibrary.Core;

namespace SecureLibrary.ExamplePlugin
{
    public class TotalClickResource : AttachedResource
    {
        public const string ResourceName = "SecureLibrary.ExamplePlugin.TotalClicks";

        public int TotalClicks { get; set; }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(TotalClicks);
        }

        public override void ReadData(BinaryReader reader)
        {
            TotalClicks = reader.ReadInt32();
        }

        public void Increase()
        {
            TotalClicks++;
        }
    }
}