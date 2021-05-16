using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IBinaryConvertedSerializable
    {
        /// <summary>
        /// Mandatory step for writing the final value
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        public void BinaryConvertedSerialize(BinaryWriter bw);
        
        public void BinaryConvertedDeserialize(BinaryReader br);
    }
}