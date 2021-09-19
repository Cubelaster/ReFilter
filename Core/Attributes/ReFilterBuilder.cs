using System;

namespace ReFilter.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ReFilterBuilder : Attribute
    {
        public Type FilterBuilderType { get; }

        public ReFilterBuilder(Type filterProviderType)
        {
            FilterBuilderType = filterProviderType;
        }
    }
}
