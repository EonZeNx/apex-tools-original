using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IBinarySerializable
    {
        /// <summary>
        /// Mandatory step for writing the final value
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        public void BinarySerialize(BinaryWriter bw);
        
        public void BinaryDeserialize(BinaryReader br);
    }
}