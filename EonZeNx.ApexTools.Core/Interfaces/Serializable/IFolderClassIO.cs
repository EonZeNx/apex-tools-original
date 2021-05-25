using EonZeNx.ApexTools.Core.Models;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IFolderClassIO : IBinarySerializable, IFolderSerializable
    {
        MetaInfo GetMetaInfo();
    }
}