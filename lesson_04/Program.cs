using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace DB_Connect
{
    class Program
    {
        static string connectionString = "Data Source=COMP11A1\\SQLEXPRESS;", "Initial Catalog=DataBase;", "Integrated Security=True;", "TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n Products ");
                Console.WriteLine("1. Show all");
                Console.WriteLine("2. Find by id");
                Console.WriteLine("3. Add");
                Console.WriteLine("4. Edit");
                Console.WriteLine("5. Delete);
                Console.WriteLine("0. Exit");
                Console.Write("You: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ListAllProducts();
                            break;
                        case "2":
                            FindProduct();
                            break;
                        case "3":
                            AddProduct();
                            break;
                        case "4":
                            UpdateProduct();
                            break;
                        case "5":
                            DeleteProduct();
                            break;
                        case "0":
                        case null:
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Errm");
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    running = false;
                }

                if (running)
                {
                    Console.WriteLine("\nPress enter");
                    Console.ReadLine();
                }
            }
        }

        static void ListAllProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT ProductID, ProductName, ProductNumber, ", "StandardCost, ListPrice, Color, BrandID ", "FROM dbo.Products ORDER BY ProductID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    bool found = false;

                    while (reader.Read())
                    {
                        PrintProduct(reader);
                        found = true;
                    }

                    if (!found)
                        Console.WriteLine("Empty");
                }
            }
        }

        static void FindProduct()
        {
            int id = ReadPositiveInt("Enter product ID: ");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT ProductID, ProductName, ProductNumber, ", "StandardCost, ListPrice, Color, BrandID ", "FROM dbo.Products WHERE ProductID = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            PrintProduct(reader);
                        else
                            Console.WriteLine("Product not found");
                    }
                }
            }
        }

        static void AddProduct()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                bool autoId;

                using (SqlCommand check = new SqlCommand(
                    "SELECT COLUMNPROPERTY(OBJECT_ID('dbo.Products'), ", "'ProductID', 'IsIdentity')", connection))
                {
                    autoId = Convert.ToInt32(check.ExecuteScalar()) == 1;
                }

                int id = 0;

                if (!autoId)
                    id = ReadPositiveInt("Enter new product ID: ");

                string sql;

                if (autoId)
                {
                    sql = "INSERT INTO dbo.Products ", "(ProductName, ProductNumber, StandardCost, ListPrice, Color, BrandID) ", "VALUES (@name, @number, @cost, @price, @color, @brand)";
                }
                else
                {
                    sql = "INSERT INTO dbo.Products ", "(ProductID, ProductName, ProductNumber, StandardCost, ListPrice, Color, BrandID) ", "VALUES (@id, @name, @number, @cost, @price, @color, @brand)";
                }

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    if (!autoId)
                        command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    ReadProductParameters(command);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Product added");
                }
            }
        }

        static void UpdateProduct()
        {
            int id = ReadPositiveInt("Enter product ID to edit: ");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand check = new SqlCommand(
                    "SELECT COUNT(*) FROM dbo.Products WHERE ProductID = @id", connection))
                {
                    check.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                    {
                        Console.WriteLine("Product not found");
                        return;
                    }
                }

                Console.WriteLine("Enter new values for all fields, ID cannot be changed");

                string sql = "UPDATE dbo.Products SET ProductName = @name, ", "ProductNumber = @number, StandardCost = @cost, ", "ListPrice = @price, Color = @color, BrandID = @brand ", "WHERE ProductID = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    ReadProductParameters(command);

                    int rows = command.ExecuteNonQuery();

                    Console.WriteLine(rows > 0 ? "Product updated" : "Product not found");
                }
            }
        }

        static void DeleteProduct()
        {
            int id = ReadPositiveInt("Enter product ID to delete: ");

            Console.Write("Confirm deletion (yes): ");
            string answer = ReadLine();

            if (!string.Equals(answer.Trim(), "yes", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Deletion cancelled");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "DELETE FROM dbo.Products WHERE ProductID = @id", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    int rows = command.ExecuteNonQuery();

                    Console.WriteLine(rows > 0 ? "Product deleted" : "Product not found");
                }
            }
        }

        static void PrintProduct(SqlDataReader reader)
        {
            string color = reader.IsDBNull(5) ? "not specified" : reader.GetString(5);

            Console.WriteLine(
                $"ID: {reader.GetInt32(0)}, " +
                $"Name: {reader.GetString(1)}, " +
                $"Number: {reader.GetString(2)}, " +
                $"Cost: {reader.GetDecimal(3):0.####}, " +
                $"Sale price: {reader.GetDecimal(4):0.####}, " +
                $"Color: {color}, BrandID: {reader.GetInt32(6)}");
        }

        static void ReadProductParameters(SqlCommand command)
        {
            string name = ReadText("Product name: ", 100, false);
            string number = ReadText("Product number: ", 40, false);
            decimal cost = ReadMoney("Cost: ");
            decimal price = ReadMoney("Sale price: ");
            string color = ReadText("Color (Enter — not specified): ", 20, true);
            int brand = ReadPositiveInt("ID of existing brand: ");

            command.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name;
            command.Parameters.Add("@number", SqlDbType.NVarChar, 40).Value = number;
            command.Parameters.Add("@cost", SqlDbType.Money).Value = cost;
            command.Parameters.Add("@price", SqlDbType.Money).Value = price;

            command.Parameters.Add("@color", SqlDbType.NVarChar, 20).Value =
                color.Length == 0 ? (object)DBNull.Value : color;

            command.Parameters.Add("@brand", SqlDbType.Int).Value = brand;
        }

        static string ReadLine()
        {
            string text = Console.ReadLine();

            if (text == null)
                throw new OperationCanceledException();

            return text;
        }

        static int ReadPositiveInt(string message)
        {
            while (true)
            {
                Console.Write(message);

                int value;

                if (int.TryParse(ReadLine(), out value) && value > 0)
                    return value;

                Console.WriteLine("Enter an integer greater than zero");
            }
        }

        static decimal ReadMoney(string message)
        {
            while (true)
            {
                Console.Write(message);

                string text = ReadLine().Trim().Replace(',', '.');
                decimal value;

                if (decimal.TryParse(text,
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out value) &&
                    value >= 0 && value <= 922337203685477.5807m &&
                    decimal.Round(value, 4) == value)
                {
                    return value;
                }

                Console.WriteLine("Enter a non-negative amount within the money range " +
                    "with a maximum of 4 decimal places (for example, 125,50)");
            }
        }

        static string ReadText(string message, int maxLength, bool allowEmpty)
        {
            while (true)
            {
                Console.Write(message);

                string text = ReadLine().Trim();

                if ((allowEmpty || text.Length > 0) && text.Length <= maxLength)
                    return text;

                Console.WriteLine("Enter " + (allowEmpty ? "no more than " : "from 1 to ") +
                    maxLength + " characters");
            }
        }
    }
}