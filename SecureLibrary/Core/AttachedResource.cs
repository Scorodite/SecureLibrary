using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Core
{
    /// <summary>
    /// Resource that can be attached to the library, saved and loaded by plugin`s feature
    /// </summary>
    public class AttachedResource : ISerializable
    {
        public virtual void ReadData(BinaryReader reader) { }
        public virtual void WriteData(BinaryWriter writer) { }
    }
}
