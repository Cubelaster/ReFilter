namespace TestProject.Models
{
    public class CountryFilterRequest
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Alpha2Code { get; set; }
    }
}
