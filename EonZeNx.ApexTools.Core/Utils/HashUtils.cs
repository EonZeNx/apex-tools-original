using System.Data.SQLite;

namespace EonZeNx.ApexTools.Core.Utils
{
    public static class HashUtils
    {
        public static string Lookup(SQLiteConnection con, int hash)
        {
            var command = con.CreateCommand();
            command.CommandText = "SELECT Value " +
                                  "FROM properties " +
                                  $"WHERE Hash = {hash}";
            using (var dbr = command.ExecuteReader())
            {
                if (dbr.Read()) return dbr.GetString(0);
                return "";
            }
        }
    }
}