using System.Collections.Generic;
using TestProject.Models;

namespace TestProject.TestData
{
    static class SchoolServiceTestData
    {
        public static List<School> Schools
        {
            get
            {
                var schoolList = new List<School>();
                for (int i = 1; i <= 100; i++)
                {
                    var newSchool = new School
                    {
                        Id = i,
                        Name = $"School Name {i}",
                        Address = $"School Address {i}",
                        Contacts = new List<string> { $"Contact {i}", $"Contact {i + 1}" },
                        Students = new List<Student>(),
                        Country = new Country
                        {
                            Id = i,
                            Name = $"Country Name {i}",
                            Alpha2Code = $"Alpha2Code {i}",
                            Description = $"Description {i}",
                        }
                    };

                    schoolList.Add(newSchool);
                }

                return schoolList;
            }
        }
    }
}
