using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace DependencyInjectionWorkshop.Repositories
{
    public interface IProfile
    {
        string GetPassword(string accountId);
    }

    public class ProfileDao : IProfile
    {
        public string GetPassword(string accountId)
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