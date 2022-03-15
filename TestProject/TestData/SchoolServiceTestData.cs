using System;
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
                        IdRange = i,
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
                        },
                        IsActive = Convert.ToBoolean(i % 2),
                        Age = i * 10 / 1.7,
                        FoundingDate = new DateTime(1900 + (i * 100) / 24, i % 12 + 1, i % 12 + 1),
                        ValidOn = new DateOnly(1900 + (i * 100) / 24, i % 12 + 1, i % 12 + 1),
                        ValidOnSingle = new DateOnly(1900 + (i * 100) / 24, i % 12 + 1, i % 12 + 1)
                    };

                    schoolList.Add(newSchool);
                }

                return schoolList;
            }
        }
    }
}
