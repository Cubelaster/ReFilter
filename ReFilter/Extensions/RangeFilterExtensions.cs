using System.Collections.Generic;
using ReFilter.Enums;
using ReFilter.Models;

namespace ReFilter.Extensions
{
    public static class RangeFilterExtensions
    {
        public static List<PropertyFilterConfig> Unpack<U>(this RangeFilter<U> rangeFilter, PropertyFilterConfig selectedPfc) where U : struct
        {
            var newPropertyFilterConfigs = new List<PropertyFilterConfig>();

            var lowValue = rangeFilter.Start;
            var highValue = rangeFilter.End;

            switch (selectedPfc.OperatorComparer)
            {
                case OperatorComparer.Equals:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = OperatorComparer.LessThanOrEqual,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });

                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = OperatorComparer.GreaterThanOrEqual,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                case OperatorComparer.BetweenExclusive:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
                case OperatorComparer.BetweenInclusive:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThanOrEqual,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThanOrEqual,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
                case OperatorComparer.BetweenHigherInclusive:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThanOrEqual,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
                case OperatorComparer.BetweenLowerInclusive:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThanOrEqual,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
                case OperatorComparer.LessThan:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = selectedPfc.OperatorComparer,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                case OperatorComparer.LessThanOrEqual:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = selectedPfc.OperatorComparer,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                case OperatorComparer.GreaterThan:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = selectedPfc.OperatorComparer,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                case OperatorComparer.GreaterThanOrEqual:
                    newPropertyFilterConfigs.Add(new PropertyFilterConfig
                    {
                        OperatorComparer = selectedPfc.OperatorComparer,
                        PropertyName = selectedPfc.PropertyName,
                        Value = lowValue ?? highValue
                    });
                    break;
                default:
                    if (lowValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = lowValue
                        });
                    }

                    if (highValue != null)
                    {
                        newPropertyFilterConfigs.Add(new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = selectedPfc.PropertyName,
                            Value = highValue
                        });
                    }
                    break;
            }

            return newPropertyFilterConfigs;
        }
    }
}
