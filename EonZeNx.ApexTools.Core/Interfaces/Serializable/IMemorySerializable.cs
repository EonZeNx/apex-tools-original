using System.IO;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IMemorySerializable
    {
        public long MemorySerializeData(MemoryStream ms, long offset);

        public void MemorySerializeHeader(MemoryStream ms);
    }
}