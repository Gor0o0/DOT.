using System;
using System.Data.SqlClient;

namespace DB_Connect_First
{
    class Program
    {
        static string connectionString = "Data Source=COMP11A1\\SQLEXPRESS;" +
            "Initial Catalog=DB1;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            bool start = true;


            while (start)
            {
                Console.WriteLine("1. Show all");
                Console.WriteLine("2. Find by id");
                Console.WriteLine("3. Exit");
                Console.Write("Enter smth: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListAllProducts();
                        break;

                    case "2":
                        FindProduct();
                        break;

                    case "`":
                        start = false;
                        break;

                    default:
                        Console.WriteLine("Errm");
                        break;
                }
                if (start)
                {
                    Console.WriteLine("\nType smth to continue");
                    Console.ReadKey();
                }
            }
        }

        static void ListAllProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql_command = "SELECT TOP 10 * FROM products";

                SqlCommand command = new SqlCommand(sql_command, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string products_number = reader.GetString(2);
                        decimal cost = reader.GetDecimal(3);
                        decimal list_price = reader.GetDecimal(4);
                        decimal diff_price = reader.GetDecimal(5);
                        DateTime delivery_date = reader.GetDateTime(6);

                        Console.WriteLine(
                            $"ID: {id}, " +
                            $"Name: {name}, " +
                            $"Prod: {products_number}, " +
                            $"Cost: {cost}, " +
                            $"LP: {list_price}, " +
                            $"DP: {diff_price}, " +
                            $"DD: {delivery_date}");
                    }
                }
            }
        }

        static void FindProduct()
        {
            Console.Write("Enter id: ");
            string id = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql_command = "SELECT * FROM products WHERE id = @id";

                SqlCommand command = new SqlCommand(sql_command, connection);

                command.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int product_id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string products_number = reader.GetString(2);
                        decimal cost = reader.GetDecimal(3);
                        decimal list_price = reader.GetDecimal(4);
                        decimal diff_price = reader.GetDecimal(5);
                        DateTime delivery_date = reader.GetDateTime(6);

                        Console.WriteLine(
                            $"Name: {name}, " +
                            $"Prod: {products_number}, " +
                            $"Cost: {cost}, " +
                            $"LP: {list_price}, " +
                            $"DD: {delivery_date}");
                    }
                    else
                    {
                        Console.WriteLine("Errm: not found");
                    }
                }
            }
        }
    }
}
