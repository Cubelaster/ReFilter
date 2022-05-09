using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ReFilter.Enums;
using ReFilter.Extensions;
using ReFilter.Models;

namespace ReFilter.ReFilterExpressionBuilder
{
    // https://stackoverflow.com/questions/23718054/dynamic-linq-building-expression
    // https://stackoverflow.com/questions/22672050/dynamic-expression-tree-to-filter-on-nested-collection-properties/22685407#22685407
    // https://stackoverflow.com/questions/536932/how-to-create-expression-tree-lambda-for-a-deep-property-from-a-string
    public class ReFilterExpressionBuilder
    {
        public List<Expression<Func<T, bool>>> BuildPredicate<T>(PropertyFilterConfig propertyFilterConfig)
        {
            var parameterExpression = Expression.Parameter(typeof(T), typeof(T).Name);
            return BuildNavigationExpression<T>(parameterExpression, propertyFilterConfig)
                    .Cast<Expression<Func<T, bool>>>()
                    .ToList();
        }

        public PropertyFilterConfig BuildSearchPropertyFilterConfig(PropertyInfo property, string searchQuery)
        {
            return new PropertyFilterConfig
            {
                OperatorComparer = OperatorComparer.Contains,
                PropertyName = property.Name,
                Value = searchQuery
            };
        }

        public PropertyInfo GetChildProperty(Expression parameter, PropertyFilterConfig propertyFilterConfig)
        {
            var sameNameProperties = parameter.Type.GetProperties()
                .Where(p => p.Name == propertyFilterConfig.PropertyName)
                .ToList();

            if (sameNameProperties.Any() && sameNameProperties.Count > 1)
            {
                var declaringTypeName = parameter.Type.Name;
                var childProperties = parameter.Type.GetProperties()
                    .Where(e => e.DeclaringType.Name == declaringTypeName)
                    .ToList();

                return childProperties.FirstOrDefault();
            }
            else
            {
                return sameNameProperties.FirstOrDefault();
            }
        }

        private List<Expression> BuildNavigationExpression<T>(Expression parameter, PropertyFilterConfig propertyFilterConfig)
        {
            PropertyInfo childProperty = GetChildProperty(parameter, propertyFilterConfig);

            if ((!childProperty.PropertyType.IsByRef && !childProperty.PropertyType.IsClass)
                || childProperty.PropertyType.IsValueType || childProperty.PropertyType.Name == "String")
            {
                // Meant to handle all strings and similar simple stuff
                return new List<Expression> { BuildCondition<T>(parameter, propertyFilterConfig) };
            }
            else if (childProperty.PropertyType.IsClass && !typeof(IEnumerable).IsAssignableFrom(childProperty.PropertyType))
            {
                // Meant to handle Recursive Search
                List<PropertyInfo> searchableProperties = childProperty.PropertyType.GetSearchableProperties();
                if (searchableProperties.Any())
                {
                    // This is key for recursion
                    var childParameter = Expression.Property(parameter, childProperty);
                    var expressions = new List<Expression>();
                    searchableProperties.ForEach(e =>
                    {
                        var newPropertyFilterConfig = BuildSearchPropertyFilterConfig(e, (string)propertyFilterConfig.Value);
                        expressions.AddRange(BuildNavigationExpression<T>(childParameter, newPropertyFilterConfig));
                    });

                    return expressions;
                }
                else
                {
                    return new List<Expression>();
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(childProperty.PropertyType))
            {
                // if it´s a collection we later need to use the predicate in the methodexpressioncall
                var childType = childProperty.PropertyType.GenericTypeArguments[0];
                List<PropertyInfo> searchableProperties = childType.GetSearchableProperties();

                if (searchableProperties.Any())
                {
                    // This is key for recursion
                    var childParameterStandalone = Expression.Parameter(childType, childType.Name);
                    var childParameter = Expression.Property(parameter, childProperty);
                    var subExpressions = new List<Expression>();

                    var childBuilderMethod = typeof(ReFilterExpressionBuilder).GetMethod(nameof(BuildNavigationExpression), BindingFlags.NonPublic | BindingFlags.Instance);
                    var childNavigationExpressionBuilder = childBuilderMethod.MakeGenericMethod(childType);
                    var newInstance = Expression.New(typeof(ReFilterExpressionBuilder));

                    searchableProperties.ForEach(e =>
                    {
                        var newPropertyFilterConfig = BuildSearchPropertyFilterConfig(e, (string)propertyFilterConfig.Value);
                        var methodCallExpression = Expression.Call(newInstance, childNavigationExpressionBuilder,
                            Expression.Constant(childParameterStandalone), Expression.Constant(newPropertyFilterConfig));
                        subExpressions.AddRange(Expression.Lambda<Func<List<Expression>>>(methodCallExpression).Compile()());
                    });

                    var childExpressions = new List<Expression>();
                    var childSubQueryBuilderMethod = typeof(ReFilterExpressionBuilder).GetMethod(nameof(BuildSubQuery), BindingFlags.NonPublic | BindingFlags.Instance);
                    var childSubQueryBuilder = childSubQueryBuilderMethod.MakeGenericMethod(childType);

                    subExpressions.ForEach(subExpression =>
                    {
                        childExpressions.AddRange(BuildSubQuery<T>(childParameter, childType, subExpression));
                    });

                    return childExpressions;
                }
                else
                {
                    return new List<Expression>();
                }
            }
            else
            {
                return new List<Expression> { BuildCondition<T>(parameter, propertyFilterConfig) };
            }
        }

        private List<Expression> BuildSubQuery<T>(Expression parameter, Type childType, Expression predicate)
        {
            var anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            anyMethod = anyMethod.MakeGenericMethod(childType);
            predicate = Expression.Call(anyMethod, parameter, predicate);
            return new List<Expression> { MakeLambda(parameter, predicate) };
        }

        private Expression BuildCondition<T>(Expression parameter, PropertyFilterConfig propertyFilterConfig)
        {
            PropertyInfo childProperty = GetChildProperty(parameter, propertyFilterConfig);

            //var childProperty = parameter.Type.GetProperty(propertyFilterConfig.PropertyName);
            var left = Expression.Property(parameter, childProperty);
            var right = Expression.Constant(propertyFilterConfig.Value);
            var predicate = BuildComparsion(left, propertyFilterConfig.OperatorComparer.Value, right);
            return MakeLambda(parameter, predicate);
        }

        private Expression BuildComparsion(Expression left, OperatorComparer comparer, Expression right)
        {
            var mask = new List<OperatorComparer>{
                OperatorComparer.Contains,
                OperatorComparer.NotContains,
                OperatorComparer.StartsWith,
                OperatorComparer.NotStartsWith,
                OperatorComparer.EndsWith,
                OperatorComparer.NotEndsWith
            };

            var rangeMask = new List<OperatorComparer>
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

            if (rangeMask.Contains(comparer))
            {

            }
            else if (!mask.Contains(comparer))
            {
                return Expression.MakeBinary((ExpressionType)comparer, left, Expression.Convert(right, left.Type));
            }

            return BuildStringCondition(left, comparer, right);
        }

        private Expression BuildStringCondition(Expression left, OperatorComparer comparer, Expression right)
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
            //var toLowerMethod = typeof(string).GetMethods()
            //    .Single(m => m.Name.Equals("ToLower") && m.GetParameters().Count() == 0);

            //left = Expression.Call(left, toLowerMethod);
            //right = Expression.Call(right, toLowerMethod);

            var toLowerSafeMethod = typeof(StringExtensions).GetMethod(nameof(StringExtensions.NullSafeToLower));

            left = Expression.Call(null, toLowerSafeMethod, left);
            right = Expression.Call(null, toLowerSafeMethod, right);
            if (isNot)
            {
                return Expression.Not(Expression.Call(left, compareMethod, right));
            }
            else
            {
                return Expression.Call(left, compareMethod, right);
            }
        }

        private Expression MakeLambda(Expression parameter, Expression predicate)
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
