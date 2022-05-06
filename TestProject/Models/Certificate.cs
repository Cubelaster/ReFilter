using ReFilter.Attributes;

namespace TestProject.Models
{
    internal class Certificate
    {
        public int Id { get; set; }
        [ReFilterProperty]
        public string Name { get; set; }
        [ReFilterProperty]
        public string Publisher { get; set; }
        [ReFilterProperty]
        public string Mark { get; set; }
    }
}
