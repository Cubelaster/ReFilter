using System.Linq.Expressions;

namespace ReFilter.Enums
{
    public enum OperatorComparer
    {
        Contains,
        StartsWith,
        EndsWith,
        Equals = ExpressionType.Equal,
        GreaterThan = ExpressionType.GreaterThan,
        GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual,
        LessThan = ExpressionType.LessThan,
        LessThanOrEqual = ExpressionType.LessThanOrEqual,
        NotEqual = ExpressionType.NotEqual,
        Not = ExpressionType.Not,
        NotStartsWith = 92,
        NotEndsWith = 93,
        NotContains = 94,
        BetweenExclusive = 95,
        BetweenInclusive = 96,
        BetweenLowerInclusive = 97,
        BetweenHigherInclusive = 98,
        CustomFilter = 99
    }
}
