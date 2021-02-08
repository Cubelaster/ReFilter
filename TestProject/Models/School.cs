using System.Collections.Generic;

namespace TestProject.Models
{
    class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public List<string> Contacts { get; set; }
        public List<Student> Students { get; set; }
    }
}
