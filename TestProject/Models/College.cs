using ReFilter.Attributes;

namespace TestProject.Models
{
    internal class College : School
    {
        [ReFilterProperty(UsedForSearchQuery = true)]
        public new string Age { get; set; }
    }
}
