using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IDeferredSerializable
    {
        /// <summary>
        /// If used, treat BinarySerialize as writing the header and BinarySerializeData as writing the data.
        /// </summary>
        /// <param name="bw"></param>
        void BinarySerializeData(BinaryWriter bw);
    }
}