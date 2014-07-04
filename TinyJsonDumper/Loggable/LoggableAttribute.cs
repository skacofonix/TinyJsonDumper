using System;

namespace TinyJsonDumper.Loggable
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class LoggableAttribute : LoggableBaseAttribute
    {
        public int? Order { get; set; }
    }
}
