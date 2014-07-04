using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TinyJsonDumper.Loggable
{
    public class LoggableDumper : TinyJsonDumper
    {
        protected override List<PropertyInfo> GetPropperties(Type t)
        {
            var properties = t.GetProperties(BindingFlags).ToList();

            List<PropertyInfo> loggableProperties;
            if (IsLoggableType(t))
            {
                loggableProperties = properties
                                        .Where(w => !w.GetCustomAttributes(typeof(LoggableIgnoreAttribute), true).Any())
                                        .ToList();
            }
            else
            {
                var loggablePropertiesUnordered = properties
                                                    .Where(w => w.GetCustomAttributes(typeof(LoggableAttribute), true).Any())
                                                    .ToList();

                loggableProperties = (from prop in loggablePropertiesUnordered
                                      let order = ((LoggableAttribute)prop.GetCustomAttributes(typeof(LoggableAttribute), true).First()).Order ?? int.MaxValue
                                      orderby order, prop.Name
                                      select prop)
                                      .ToList();
            }

            return loggableProperties;

        }

        private static bool IsLoggableType(Type t)
        {
            return t.GetCustomAttributes(typeof(LoggableAttribute), true).Any();
        }
    }
}
