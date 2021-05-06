using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IBinarySerializable
    {
        /// <summary>
        /// Mandatory step for writing the final value
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        public void BinarySerialize(BinaryWriter bw) {}
        
        // TODO: Read all properties then deserialize, this will half the number of seeks to perform
        public void BinaryDeserialize(BinaryReader br) {}
    }
}