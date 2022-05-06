using ReFilter.Attributes;

namespace TestProject.Models
{
    internal class Building
    {
        public int Id { get; set; }
        [ReFilterProperty]
        public string Name { get; set; }
        public int Year { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        [ReFilterProperty]
        public string Orientation { get; set; }
        [ReFilterProperty]
        public string BuiltBy { get; set; }
    }
}
