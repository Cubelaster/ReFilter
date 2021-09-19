using System.Collections.Generic;
using ReFilter.Core.Attributes;
using ReFilter.Core.Models.Filtering.Contracts;

namespace TestProject.Models.FilterRequests
{
    class SchoolFilterRequest : IReFilterRequest
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public List<string> Contacts { get; set; }
        [ReFilterProperty(HasSpecialFilter = true)]
        public List<Student> Students { get; set; }
    }
}
