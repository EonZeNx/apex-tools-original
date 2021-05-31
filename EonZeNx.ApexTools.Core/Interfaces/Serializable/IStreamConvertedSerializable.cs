using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IStreamConvertedSerializable
    {
        /// <summary>
        /// Mandatory step for writing the final value
        /// </summary>
        /// <param name="s"></param>
        public void StreamConvertedSerialize(Stream s);
        
        public void StreamConvertedDeserialize(Stream s);
    }
}