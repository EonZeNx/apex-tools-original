using EonZeNx.ApexTools.Core.Models;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IFolderClassIO : IStreamSerializable, IFolderSerializable
    {
        MetaInfo GetMetaInfo();
    }
}