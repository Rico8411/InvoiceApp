
using System;
using System.Data;
using System.Data.SqlClient;


namespace InvoiceApp.DataAccess
{
    internal class InvoiceRepository
    {
        public int GetNextInvoiceNumber()
        {
            using (SqlConnection connection = Database.OpenConnection())
            using (SqlCommand command =
                Database.CreateStoredProcedureCommand(
                    connection, "GetNextInvoiceNumber"))
            {
                object result = command.ExecuteScalar();

                return Convert.ToInt32(result);
            }
        }
        public int InsertInvoice(
        int invoiceNumber,
        DateTime invoiceDate,
        TimeSpan invoiceTime,
        string customerName,
        int customerId)
        {
            using (SqlConnection connection = Database.OpenConnection())
            using (SqlCommand command =
                Database.CreateStoredProcedureCommand(
                    connection, "InsertInvoice"))
            {
                command.Parameters.Add("@InvoiceNumber", SqlDbType.Int)
                    .Value = invoiceNumber;

                command.Parameters.Add("@InvoiceDate", SqlDbType.Date)
                    .Value = invoiceDate.Date;

                command.Parameters.Add("@InvoiceTime", SqlDbType.Time)
                    .Value = invoiceTime;

                command.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 100)
                    .Value = customerName;

                command.Parameters.Add("@CustomerId", SqlDbType.Int)
                    .Value = customerId;

                object result = command.ExecuteScalar();

                return Convert.ToInt32(result);
            }
        }
    }
}
