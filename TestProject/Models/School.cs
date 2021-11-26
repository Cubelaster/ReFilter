using System.Collections.Generic;
using ReFilter.Attributes;

namespace TestProject.Models
{
    class School
    {
        public int Id { get; set; }
        [ReFilterProperty]
        public string Name { get; set; }
        [ReFilterProperty]
        public string Address { get; set; }

        public Country Country { get; set; }

        public List<string> Contacts { get; set; }
        public List<Student> Students { get; set; }

        public double Age { get; set; }

        public bool IsActive { get; set; }
    }
}
