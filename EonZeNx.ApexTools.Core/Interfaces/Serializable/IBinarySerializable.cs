using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IBinarySerializable
    {
        public void BinarySerialize(BinaryWriter bw) {}
        public void BinaryDeserialize(BinaryReader br) {}
    }
}