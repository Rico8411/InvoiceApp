using System.Data;
using System.Data.SqlClient;

namespace InvoiceApp
{
    internal static class Database
    {
        private const string ConnectionString =
            "Server=.;Database=InvoiceAppDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);

            try
            {
                connection.Open();
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        public static SqlCommand CreateStoredProcedureCommand(
            SqlConnection connection,
            string procedureName)
        {
            SqlCommand command = new SqlCommand(procedureName, connection);
            command.CommandType = CommandType.StoredProcedure;
            return command;
        }
    }
}