using System;
using System.Collections.Generic;
using TestProject.Models;

namespace TestProject.TestData
{
    static class StudentServiceTestData
    {
        public static List<Student> Students
        {
            get
            {
                var studentList = new List<Student>();
                for (int i = 1; i <= 100; i++)
                {
                    var newStudent = new Student
                    {
                        Id = i,
                        Age = new Random().Next(0, 100),
                        FirstName = $"FirstName {i}",
                        LastName = $"LastName {i}",
                        Gender = (Enums.Gender)(i%3)
                    };

                    studentList.Add(newStudent);
                }

                return studentList;
            }
        }
    }
}
