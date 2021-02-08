using System.Collections;
using System.Collections.Generic;
using ReFilter.Models;
using Tests.Models;

namespace Tests.TestData
{
    public class StudentServiceTestData : IEnumerable<object[]>
    {
        static List<Student> Students = new List<Student>
        {
            new Student
            {
                Id = 1,
                Age = 2,
                FirstName = "First",
                LastName = "First LastName",
                Gender = Enums.Gender.Undisclosed
            },
            new Student
            {
                Id = 2,
                Age = 34,
                FirstName = "Second",
                LastName = "Second LastName",
                Gender = Enums.Gender.Male
            },
            new Student
            {
                Id = 3,
                Age = 42,
                FirstName = "Third",
                LastName = "Third LastName",
                Gender = Enums.Gender.Female
            }
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 2
                },
                Students.Count
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
