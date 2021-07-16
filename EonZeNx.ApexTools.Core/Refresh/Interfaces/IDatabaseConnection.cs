using System.Data.SQLite;

namespace EonZeNx.ApexTools.Core.Refresh.Interfaces
{
    public interface IDatabaseConnection
    {
        SQLiteConnection DbConnection { get; set; }
    }
}