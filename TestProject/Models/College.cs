using ReFilter.Attributes;

namespace TestProject.Models
{
    internal class College : School
    {
        [ReFilterProperty]
        public new string Age { get; set; }
    }
}
