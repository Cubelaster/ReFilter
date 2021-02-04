using System;

namespace ReFilter.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ReFilterSpecialFilter : Attribute
    {
        public string AttributeName { get; set; }
    }
}
