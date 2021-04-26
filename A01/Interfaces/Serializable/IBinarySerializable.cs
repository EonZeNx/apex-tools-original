using System.IO;

namespace A01.Interfaces.Serializable
{
    public interface IBinarySerializable
    {
        public void BinarySerialize(BinaryWriter bw) {}
        public void BinaryDeserialize(BinaryReader br) {}
    }
}