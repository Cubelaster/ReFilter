using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReFilter.Enums;
using ReFilter.Models;

namespace ReFilter.ReFilterExpressionBuilder
{
    public class ReFilterExpressionBuilder
    {
        public static Expression<Func<T, bool>> BuildPredicate<T>(PropertyFilterConfig propertyFilterConfig)
        {
            var parameterExpression = Expression.Parameter(typeof(T), typeof(T).Name);
            return (Expression<Func<T, bool>>)BuildNavigationExpression(parameterExpression, propertyFilterConfig);
        }

        private static Expression BuildNavigationExpression(Expression parameter, PropertyFilterConfig propertyFilterConfig)
        {
            var isCollection = typeof(IEnumerable).IsAssignableFrom(parameter.Type);

            //if it´s a collection we later need to use the predicate in the methodexpressioncall
            if (isCollection)
            {
                var childType = parameter.Type.GetGenericArguments()[0];
                var childParameter = Expression.Parameter(childType, childType.Name);
                var newPredicate = BuildNavigationExpression(childParameter, propertyFilterConfig);

                return BuildSubQuery(childParameter, childType, newPredicate);
            }

            return BuildCondition(parameter, propertyFilterConfig);
        }

        private static Expression BuildSubQuery(Expression parameter, Type childType, Expression predicate)
        {
            var anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            anyMethod = anyMethod.MakeGenericMethod(childType);
            predicate = Expression.Call(anyMethod, parameter, predicate);
            return MakeLambda(parameter, predicate);
        }

        private static Expression BuildCondition(Expression parameter, PropertyFilterConfig propertyFilterConfig)
        {
            var childProperty = parameter.Type.GetProperty(propertyFilterConfig.PropertyName);
            var left = Expression.Property(parameter, childProperty);
            var right = Expression.Constant(propertyFilterConfig.Value);
            var predicate = BuildComparsion(left, propertyFilterConfig.OperatorComparer.Value, right);
            return MakeLambda(parameter, predicate);
        }

        private static Expression BuildComparsion(Expression left, OperatorComparer comparer, Expression right)
        {
            var mask = new List<OperatorComparer>{
                OperatorComparer.Contains,
                OperatorComparer.NotContains,
                OperatorComparer.StartsWith,
                OperatorComparer.NotStartsWith,
                OperatorComparer.EndsWith,
                OperatorComparer.NotEndsWith
            };

            var numericMask = new List<OperatorComparer>
            {
                OperatorComparer.BetweenExclusive,
                OperatorComparer.BetweenInclusive,
                OperatorComparer.BetweenHigherInclusive,
                OperatorComparer.BetweenLowerInclusive
            };

            if (mask.Contains(comparer) && left.Type != typeof(string))
            {
                comparer = OperatorComparer.Equals;
            }

            if (!mask.Contains(comparer))
            {
                return Expression.MakeBinary((ExpressionType)comparer, left, Expression.Convert(right, left.Type));
            }

            return BuildStringCondition(left, comparer, right);
        }

        private static Expression BuildStringCondition(Expression left, OperatorComparer comparer, Expression right)
        {
            var isNot = false;
            var operatorName = Enum.GetName(typeof(OperatorComparer), comparer);
            if (operatorName.Contains("Not"))
            {
                isNot = true;
                operatorName = operatorName.Replace("Not", "");
            }

            // Single or first, we'll need to debug
            var compareMethod = typeof(string).GetMethods()
                .Single(m => m.GetParameters().Any(p => p.ParameterType == typeof(string))
                    && m.Name.Equals(operatorName) && m.GetParameters().Count() == 1);
            //we assume ignoreCase, so call ToLower on paramter and memberexpression
            var toLowerMethod = typeof(string).GetMethods()
                .Single(m => m.Name.Equals("ToLower") && m.GetParameters().Count() == 0);
            left = Expression.Call(left, toLowerMethod);
            right = Expression.Call(right, toLowerMethod);
            if (isNot)
            {
                return Expression.Not(Expression.Call(left, compareMethod, right));
            }
            else
            {
                return Expression.Call(left, compareMethod, right);
            }
        }

        private static Expression MakeLambda(Expression parameter, Expression predicate)
        {
            var resultParameterVisitor = new ParameterVisitor();
            resultParameterVisitor.Visit(parameter);
            var resultParameter = resultParameterVisitor.Parameter;
            return Expression.Lambda(predicate, (ParameterExpression)resultParameter);
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            public Expression Parameter
            {
                get;
                private set;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                Parameter = node;
                return node;
            }
        }
    }
}
