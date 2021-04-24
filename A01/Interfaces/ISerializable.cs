using System.IO;

namespace A01.Interfaces
{
    public interface ISerializable
    {
        public void Serialize(BinaryWriter bw) {}
        public void Deserialize(BinaryReader br) {}
    }
}