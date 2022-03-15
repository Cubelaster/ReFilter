using System;
using System.Collections.Generic;
using ReFilter.Attributes;
using TestProject.FilterBuilders;

namespace TestProject.Models
{
    [ReFilterBuilder(typeof(SchoolFilterBuilder))]
    class School
    {
        public int Id { get; set; }
        public int IdRange { get; set; }
        [ReFilterProperty]
        public string Name { get; set; }
        [ReFilterProperty]
        public string Address { get; set; }

        public Country Country { get; set; }

        public List<string> Contacts { get; set; }
        public List<Student> Students { get; set; }

        public double Age { get; set; }
        public DateTime FoundingDate { get; set; }
        public DateOnly ValidOn { get; set; }
        public DateOnly ValidOnSingle { get; set; }

        public bool IsActive { get; set; }
    }
}
