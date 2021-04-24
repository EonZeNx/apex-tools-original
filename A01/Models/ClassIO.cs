using System.IO;
using A01.Interfaces;

namespace A01.Models
{
    public abstract class ClassIO : ISerializable
    {
        public abstract string FileType { get; set; }
        public abstract int Version { get; set; }
        public abstract string Extension { get; set; }

        public abstract void Serialize(BinaryWriter bw);
        public abstract void Deserialize(BinaryReader br);
    }
}