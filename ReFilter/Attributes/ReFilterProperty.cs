using System;

namespace ReFilter.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ReFilterProperty : Attribute
    {
        public bool UsedForSearchQuery { get; set; } = true;
        public bool HasSpecialFilter { get; set; } = false;
    }
}
