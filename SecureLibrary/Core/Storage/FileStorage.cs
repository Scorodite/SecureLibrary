using System.IO;

namespace SecureLibrary.Core.Storage
{
    public class FileStorage : BinaryStorage
    {
        public FileStorage(string fileName)
        {
            FileName = fileName;
        }

        public override string Name => "File - " + Path.GetFileName(FileName);

        public string FileName { get; }

        public override Stream OpenWrite()
        {
            return File.Exists(FileName) ? File.Open(FileName, FileMode.Truncate) : File.Create(FileName);
        }

        public override Stream OpenRead()
        {
            return File.OpenRead(FileName);
        }
    }
}
