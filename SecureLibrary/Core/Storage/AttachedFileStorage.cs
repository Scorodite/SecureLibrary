using SecureLibrary.Utilities.Streams;
using System.IO;
using System.Windows.Input;

namespace SecureLibrary.Core.Storage
{
    /// <summary>
    /// Stores library data at the end of original file data
    /// </summary>
    public class AttachedFileStorage : BinaryStorage
    {
        public AttachedFileStorage(string filename)
        {
            FileName = filename;
        }

        public override string Name => "Attached - " + Path.GetFileName(FileName);

        public string FileName { get; }

        public override Stream OpenRead()
        {
            return new AttachedStream(File.OpenRead(FileName), true);
        }

        public override Stream OpenWrite()
        {
            return new AttachedStream(
                new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), false
            );
        }
    }
}
