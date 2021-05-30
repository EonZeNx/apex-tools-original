using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IStreamSerializable
    {
        /// <summary>
        /// Mandatory step for writing the final value
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        public void StreamSerialize(BinaryWriter bw);
        
        public void StreamDeserialize(BinaryReader br);
    }
}