
using InvoiceApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace InvoiceApp.DataAccess
{
    internal class CustomerRepository
    {
        public List<Customer> GetCustomers(string searchText = "")
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection connection = Database.OpenConnection())
            using (SqlCommand command =
                Database.CreateStoredProcedureCommand(
                    connection, "SearchCustomers"))
            {
                command.Parameters.Add("@SearchText", SqlDbType.NVarChar, 100)
                    .Value = searchText;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Customer customer = new Customer(
                            Convert.ToInt32(reader["Id"]),
                            reader["FirstName"].ToString(),
                            reader["LastName"].ToString()
                        );

                        customers.Add(customer);
                    }
                }
            }

            return customers;
        }
    }
}
