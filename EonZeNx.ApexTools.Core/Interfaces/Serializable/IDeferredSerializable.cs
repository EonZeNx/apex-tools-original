using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IDeferredSerializable
    {
        /// <summary>
        /// If used, treat StreamSerialize as writing the header and StreamSerializeData as writing the data.
        /// </summary>
        /// <param name="s"></param>
        void StreamSerializeData(Stream s);
    }
}