
using System.Data;
using System.Data.SqlClient;

namespace InvoiceApp.DataAccess
{
    internal class InvoiceItemRepository
    {
        public void InsertInvoiceItem(
            int invoiceId,
            int productId,
            int quantity,
            decimal unitPrice,
            decimal amount)
        {
            using (SqlConnection connection = Database.OpenConnection())
            using (SqlCommand command =
                Database.CreateStoredProcedureCommand(
                    connection, "InsertInvoiceItem"))
            {
                command.Parameters.Add("@InvoiceId", SqlDbType.Int)
                    .Value = invoiceId;

                command.Parameters.Add("@ProductId", SqlDbType.Int)
                    .Value = productId;

                command.Parameters.Add("@Quantity", SqlDbType.Int)
                    .Value = quantity;

                command.Parameters.Add(
                    Database.CreateDecimalParameter(
                        "@UnitPrice", unitPrice));

                command.Parameters.Add(
                    Database.CreateDecimalParameter(
                        "@Amount", amount));

                command.ExecuteNonQuery();
            }
        }
    }
}
