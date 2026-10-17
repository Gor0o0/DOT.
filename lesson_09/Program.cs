using System;
using College_DB.Data;
using Microsoft.Data.SqlClient;

namespace College_DB
{
    class Program
    {
        static string conn_str = "Data Source=COMP11A1\\SQLEXPRESS;" + "Initial Catalog=College;" + "Integrated Security=True;" + "TrustServerCertificate=True;";

        static bool is_running = true;

        static StudentRepository student_repo = new StudentRepository(conn_str);

        static GroupRepository group_repo = new GroupRepository(conn_str);

        static void Main()
        {
            while (is_running)
            {
                Console.WriteLine("1. Show all students");
                Console.WriteLine("2. Show all groups");
                Console.WriteLine("3. Find by ID");
                Console.WriteLine("4. Add");
                Console.WriteLine("5. ");
                Console.WriteLine("0. Exit");
                Console.Write("You: ");

                string choise = Console.ReadLine();

                switch (choise)
                {
                    case "1":
                        ShowAllStudents();
                        break;
                    case "2":
                        ShowAllGroups();
                        break;
                    case "3":
                        ShowStudentById();
                        break;
                    case "4":
                        AddStudent();
                        break;
                    case "0":
                        is_running = false;
                        break;
                    default:
                        Console.WriteLine("No such command");
                        break;
                }

                if (is_running)
                {
                    Console.WriteLine("Press any key");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void ShowAllStudents()
        {
            var students = student_repo.GetAllStudents();

            foreach (var student in students)
            {
                Console.WriteLine(student.ToString());
            }
        }

        static void ShowAllGroups()
        {
            var groups = group_repo.GetAllGroups();

            foreach (var group in groups)
            {
                Console.WriteLine(group.ToString());
            }
        }

        static void ShowStudentById()
        {
            Console.WriteLine("Enter student ID: ");
            string choise = Console.ReadLine();

            var student = student_repo.GetStudentById(choise);

            Console.WriteLine(student.ToString());
        }

        static void AddStudent()
        {
            Console.WriteLine("Enter first name: ");
            string first_name = Console.ReadLine();

            Console.WriteLine("Enter last name: ");
            string last_name = Console.ReadLine();

            Console.WriteLine("Enter age: ");
            string age = Console.ReadLine();

            Console.WriteLine("Enter group ID: ");
            string group_id = Console.ReadLine();

            student_repo.CreateStudent(first_name, last_name, age, group_id);
            ShowAllStudents();
        }
    }
}