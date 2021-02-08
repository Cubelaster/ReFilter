﻿using TestProject.Enums;

namespace TestProject.Models
{
    class StudentViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public Gender Gender { get; set; }

        public string FullName { get; set; }
    }
}
