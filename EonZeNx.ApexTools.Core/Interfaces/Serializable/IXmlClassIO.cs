using System.Data.SQLite;
using EonZeNx.ApexTools.Core.Models;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IXmlClassIO : IStreamSerializable, IXmlSerializable
    {
        MetaInfo GetMetaInfo();
        SQLiteConnection DbConnection { get; set; }
    }
}