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
                        Students = new List<Student>
                        {
                            new Student
                            {
                                Id = i,
                                FirstName = $"Student Name School Name {i}",
                                LastName = $"Student LastName School Name {i}",
                                Age = i,
                                Gender = Enums.Gender.Male,
                            }
                        },
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
                        ValidOnSingle = new DateOnly(1900 + (i * 100) / 24, i % 12 + 1, i % 12 + 1),
                        Building = new Building
                        {
                            Id = i,
                            Name = $"Building Name {i}",
                            BuiltBy = $"Builder {i}",
                            Year = 1900 + i
                        },
                        Certificates = new List<Certificate>
                        {
                            new Certificate
                            {
                                Id = i,
                                Mark = $"C1{i}",
                                Name = $"Certificate1 {i}",
                                Publisher = $"Publisher1 {i}"
                            },
                            new Certificate
                            {
                                Id = i,
                                Mark = $"C2{i}",
                                Name = $"Certificate2 {i}",
                                Publisher = $"Publisher2 {i}"
                            }
                        }
                    };

                    schoolList.Add(newSchool);
                }

                return schoolList;
            }
        }
    }
}
