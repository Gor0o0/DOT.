using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_Connect_First
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=COMP11A1\\SQLEXPRESS;" +
                "Initial Catalog=DB1;" +
                "Integrated Security=True;" +
                "TrustServerCertificate=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string sql_command = "SELECT TOP 10 * FROM products";
            SqlCommand command = new SqlCommand(sql_command, connection);

            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string products_number = reader.GetString(2);
                decimal cost = reader.GetDecimal(3);
                decimal list_price = reader.GetDecimal(4);
                decimal diff_price = reader.GetDecimal(5);
                DateTime delivery_date = reader.GetDateTime(6);

                Console.WriteLine($"Indetify: {id}, Name: {name}, PN: {products_number}, Cost: {cost}, LP: {list_price}, DP: {diff_price}, DD: {delivery_date}");
            }

            Console.WriteLine("Hi!");
            Console.ReadKey();
        }
    }
}
