using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace College_DB.Models
{
    public class Student
    {

        static int StudentId { get; set; }
        static string FirstName { get; set; }
        static string LastName { get; set; }

        static int Age { get; set; }
        
        public override string ToString()
        {
            return $"|{StudentId}| {FirstName}| {LastName}| {Age}| ";
        }
    }
}
