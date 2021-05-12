using System.Data.SQLite;
using EonZeNx.ApexTools.Core.Models;

namespace EonZeNx.ApexTools.Core.Interfaces.Serializable
{
    public interface IXmlClassIO : IBinarySerializable, IXmlSerializable
    {
        MetaInfo GetMetaInfo();
        SQLiteConnection DbConnection { get; set; }
    }
}