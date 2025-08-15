// Dosya Adı: OgrenciPortalApi/Models/OgrenciPortalApiDB.Partial.cs

using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace OgrenciPortalApi.Models
{
    public partial class OgrenciPortalApiDB
    {

        public OgrenciPortalApiDB() : base(BuildConnectionString())
        {
        }

        private static string BuildConnectionString()
        {
            string baseConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["OgrenciPortalApiDB"].ConnectionString;

            var entityBuilder = new EntityConnectionStringBuilder(baseConnectionString);

            var sqlBuilder = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString);

            var dbPassword = DotNetEnv.Env.GetString("SQL_PASSWORD");

            sqlBuilder.Password = dbPassword;

            entityBuilder.ProviderConnectionString = sqlBuilder.ToString();

            return entityBuilder.ToString();
        }
    }
}