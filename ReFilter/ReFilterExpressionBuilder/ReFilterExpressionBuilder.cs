using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

            var left = Expression.Property(parameter, childProperty);

            if (!TryCoerceValue(propertyFilterConfig.Value, childProperty.PropertyType, out var coercedValue))
            {
                // Value can't be represented as the property's type (e.g. a search term that isn't a
                // valid Guid/number/enum for this property, or a filter value of the wrong shape).
                // Treat it as a condition that can never match rather than throwing - this is used both
                // for explicit filters (a bad value should just filter out everything, not 500) and for
                // free-text search (a term that doesn't fit this property's type should just not match
                // it, not abort matching every other property).
                return MakeLambda(parameter, Expression.Constant(false));
            }

            var right = Expression.Constant(coercedValue, childProperty.PropertyType);
            var predicate = BuildComparsion(left, propertyFilterConfig.OperatorComparer.Value, right);
            return MakeLambda(parameter, predicate);
        }

        // Value can arrive as whatever CLR type the caller put on the PFC (e.g. a raw string from a
        // query-string-bound filter, or free search text) targeting a numeric/enum/date property.
        // Expression.Convert only bridges numeric widening, boxing and Nullable<T> wrapping - it can't
        // parse a string into a number/enum/Guid/date, so we coerce the value up front and build the
        // Constant with the property's exact type. Returns false (no exception) when the value cannot
        // be coerced, so the caller can treat the condition as a non-match instead of failing outright.
        private bool TryCoerceValue(object value, Type targetType, out object coercedValue)
        {
            if (value == null)
            {
                coercedValue = null;
                return true;
            }

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType.IsInstanceOfType(value))
            {
                coercedValue = value;
                return true;
            }

            try
            {
                if (underlyingType.IsEnum)
                {
                    coercedValue = value is string enumString
                        ? Enum.Parse(underlyingType, enumString, ignoreCase: true)
                        : Enum.ToObject(underlyingType, value);
                    return true;
                }

                if (underlyingType == typeof(Guid))
                {
                    coercedValue = Guid.Parse(value.ToString());
                    return true;
                }

                if (underlyingType == typeof(DateOnly))
                {
                    coercedValue = DateOnly.Parse(value.ToString(), CultureInfo.InvariantCulture);
                    return true;
                }

                if (underlyingType == typeof(TimeOnly))
                {
                    coercedValue = TimeOnly.Parse(value.ToString(), CultureInfo.InvariantCulture);
                    return true;
                }

                if (value is IConvertible)
                {
                    coercedValue = Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
                    return true;
                }
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException or ArgumentException)
            {
                coercedValue = null;
                return false;
            }

            // Not assignable and no recognized coercion path (e.g. a non-IConvertible custom type) -
            // a mismatch, not something worth guessing at.
            coercedValue = null;
            return false;
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
            var toLowerMethod = typeof(string).GetMethods()
                .Single(m => m.Name.Equals("ToLower") && m.GetParameters().Count() == 0);

            left = Expression.Call(Expression.Coalesce(left, Expression.Constant(string.Empty)), toLowerMethod);
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
