
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InvoiceApp.Models;

namespace InvoiceApp.DataAccess
{
    internal class ProductRepository
    {
        public List<Product> GetProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = Database.OpenConnection())
            using (SqlCommand command =
                Database.CreateStoredProcedureCommand(
                    connection, "GetProducts"))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product(
                            Convert.ToInt32(reader["Id"]),
                            reader["Name"].ToString(),
                            Convert.ToDecimal(reader["Price"])
                        );

                        products.Add(product);
                    }
                }
            }


            return products;
        }

        public Product GetProductById(int productId)
        {
            using (SqlConnection connection = Database.OpenConnection())
            using (SqlCommand command =
                Database.CreateStoredProcedureCommand(
                    connection, "GetProductById"))
            {
                command.Parameters.AddWithValue("@Id", productId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Product(
                            Convert.ToInt32(reader["Id"]),
                            reader["Name"].ToString(),
                            Convert.ToDecimal(reader["Price"])
                        );
                    }
                }
            }

            return null;
        }
        public Product GetProductByName(string productName)
        {
            using (SqlConnection connection = Database.OpenConnection())
            using (SqlCommand command =
                Database.CreateStoredProcedureCommand(
                    connection, "GetProductByName"))
            {
                command.Parameters.AddWithValue("@Name", productName);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Product(
                            Convert.ToInt32(reader["Id"]),
                            reader["Name"].ToString(),
                            Convert.ToDecimal(reader["Price"])
                        );
                    }
                }
            }

            return null;
        }
    }
}
