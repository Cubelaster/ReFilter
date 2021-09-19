using ReFilter.Core.Enums;

namespace ReFilter.Core.Models
{
    public class PropertyFilterConfig
    {
        public string PropertyName { get; set; }
        public OperatorComparer? OperatorComparer { get; set; } = Enums.OperatorComparer.Equals;
        public SortDirection? SortDirection { get; set; }
        public object Value { get; set; }
    }
}
