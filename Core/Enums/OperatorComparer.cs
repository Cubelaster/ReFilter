using System.Linq.Expressions;

namespace ReFilter.Core.Enums
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
        BetweenExclusive = 95,
        BetweenInclusive = 96,
        BetweenLowerInclusive = 97,
        BetweenHigherInclusive = 98,
        CustomFilter = 99
    }
}
