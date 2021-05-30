using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IStreamConvertedSerializable
    {
        /// <summary>
        /// Mandatory step for writing the final value
        /// </summary>
        /// <param name="bw">BinaryWriter</param>
        public void StreamConvertedSerialize(BinaryWriter bw);
        
        public void StreamConvertedDeserialize(BinaryReader br);
    }
}