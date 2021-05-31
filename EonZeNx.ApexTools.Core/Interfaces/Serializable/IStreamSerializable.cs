using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IStreamSerializable
    {
        /// <summary>
        /// Mandatory step for writing the final value
        /// </summary>
        /// <param name="s"></param>
        public void StreamSerialize(Stream s);
        
        public void StreamDeserialize(Stream s);
    }
}