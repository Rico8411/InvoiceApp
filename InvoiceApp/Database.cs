using System.Data.SqlClient;

namespace InvoiceApp
{
    internal static class Database
    {
        private static readonly string connectionString =
            "Server=.;Database=InvoiceAppDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
