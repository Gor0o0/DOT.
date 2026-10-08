using System;
using Microsoft.Data.SqlClient;

namespace DB_Connect_First
{
    class Program2
    {
        static string conn_str = Connect();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1 - Add stud");
                Console.WriteLine("2 - Add group");
                Console.WriteLine("3 - Show all stud");
                Console.WriteLine("4 - Show stud from group");
                Console.WriteLine("0 - Exit");
                Console.Write("Choose action: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter first name: ");
                        string firstName = Console.ReadLine();
                        Console.Write("Enter last name: ");
                        string lastName = Console.ReadLine();
                        Console.Write("Enter age: ");
                        string age = Console.ReadLine();
                        Console.Write("Enter group ID: ");
                        string groupId = Console.ReadLine();
                        AddStudents(conn_str, firstName, lastName, age, groupId);
                        Console.WriteLine("Student added.");
                        break;

                    case "2":
                        Console.Write("Enter group name: ");
                        string groupName = Console.ReadLine();
                        AddGroup(conn_str, groupName);
                        Console.WriteLine("Group added.");
                        break;

                    case "3":
                        ShowAllStudent(conn_str);
                        break;

                    case "4":
                        Console.Write("Enter group ID: ");
                        int groupIdForSearch = int.Parse(Console.ReadLine());
                        ShowStudentFromGroup(conn_str, groupIdForSearch);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static string Connect()
        {
            string connectionString =
            "Data Source=COMP11A1\\SQLEXPRESS;" +
            "Initial Catalog=College_DB;" +
            "Integrated Security=True;" +
            "TrustServerCertificate=True;";

            return connectionString;
        }

        static void AddGroup(string conn_str, string name)
        {
            SqlConnection connection = new SqlConnection(conn_str);
            connection.Open();
            string sql = "INSERT INTO dbo.Groups (GroupName) VALUES (@name)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name);
            command.ExecuteNonQuery();
            connection.Close();

        }
        static void AddStudents(string conn_str, string first_name, string last_name, string age, string group_id)
        {
            SqlConnection connection = new SqlConnection(conn_str);
            connection.Open();
            string sql = "INSERT INTO dbo.Students(FirstName, LastName, Age, GroupId)" +
                "VALUES(@firstname, @lastName, @age, @groupid)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@firstname", first_name);
            command.Parameters.AddWithValue("@lastName", last_name);
            command.Parameters.AddWithValue("@age", age);
            command.Parameters.AddWithValue("@groupid", group_id);
            command.ExecuteNonQuery();
            connection.Close();

        }
        static void ShowAllStudent(string conn_str)
        {
            SqlConnection connection = new SqlConnection(conn_str);
            connection.Open();
            string sql = "SELECT * FROM dbo.Students";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string first_name = reader.GetString(1);
                string last_name = reader.GetString(2);
                int age = reader.GetInt32(3);
                int group_id = reader.GetInt32(4);

                Console.WriteLine($" {id} | {first_name} | {last_name} | {age} | {group_id}");


            }
            connection.Close();
        }
        static void ShowStudentFromGroup(string conn_str, int id)
        {
            SqlConnection connection = new SqlConnection(conn_str);
            connection.Open();
            string sql = "SELECT s.StudentId, s.LastName, s.FirstName" +
                "FROM dbo.Students AS s" +
                "WHERE s.GroupId = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}