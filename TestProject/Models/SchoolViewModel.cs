using System.Collections.Generic;

namespace TestProject.Models
{
    class SchoolViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public List<string> Contacts { get; set; }
        public List<StudentViewModel> Students { get; set; }
    }
}
