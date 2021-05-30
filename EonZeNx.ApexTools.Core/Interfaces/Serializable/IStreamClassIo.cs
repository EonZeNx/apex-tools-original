using EonZeNx.ApexTools.Core.Models;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IStreamClassIo : IStreamSerializable, IStreamConvertedSerializable
    {
        MetaInfo GetMetaInfo();
    }
}