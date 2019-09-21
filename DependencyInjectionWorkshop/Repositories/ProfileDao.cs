using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DependencyInjectionWorkshop.Repositories
{
    public class ProfileDao
    {
        public string GetHashedPasswordFromDb(string accountId)
        {
            string hashedPasswordFromDb;
            using (var connection = new SqlConnection("my connection string"))
            {
                hashedPasswordFromDb = connection.Query<string>("spGetUserPassword", new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            }

            return hashedPasswordFromDb;
        }
    }
}