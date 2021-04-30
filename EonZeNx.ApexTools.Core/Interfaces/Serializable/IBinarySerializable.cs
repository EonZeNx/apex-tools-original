using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IBinarySerializable
    {
        public void BinarySerialize(BinaryWriter bw) {}
        // TODO: Read all properties then deserialize, this will half the number of seeks to perform
        public void BinaryDeserialize(BinaryReader br) {}
    }
}