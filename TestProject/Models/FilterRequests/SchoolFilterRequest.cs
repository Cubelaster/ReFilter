using System;
using System.Collections.Generic;
using ReFilter.Attributes;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;

namespace TestProject.Models.FilterRequests
{
    class SchoolFilterRequest : IReFilterRequest
    {
        public int? Id { get; set; }
        public RangeFilter<int> IdRange { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        [ReFilterProperty(HasSpecialSort = true)]
        public CountryFilterRequest Country { get; set; }

        public List<string> Contacts { get; set; }
        [ReFilterProperty(HasSpecialFilter = true)]
        public List<Student> Students { get; set; }

        public RangeFilter<double> Age { get; set; }

        public RangeFilter<DateTime> FoundingDate { get; set; }
        public RangeFilter<DateOnly> ValidOn { get; set; }

        public bool? IsActive { get; set; }
    }
}
