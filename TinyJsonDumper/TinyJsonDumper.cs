using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TinyJsonDumper
{
    /// <summary>
    /// Serializer for converting object to string
    /// </summary>
    public class TinyJsonDumper
    {
        #region Const

        private const string NULL = "NULL";
        private const string EQUAL = "=";
        private const string PREFIX_OBJECT = "{";
        private const string SUFIX_OBJECT = "}";
        private const string PREFIX_ARRAY = "[";
        private const string SUFIX_ARRAY = "]";
        private const string NAME_VALUE_DELIMITER = ":";
        private const string DELIMITER = ",";
        private const string STRING_DELIMITER = "\"";
        private const string UNKNOW = "?";
        private const string INDENT = "  ";
        private const string CR = "\n";
        private const string OUT_OF_DEPTH = "#";
        private const UInt16 DEFAULT_DEPTH = 3;
        private const BindingFlags DEFAULT_BINDINGFLAGS = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;

        #endregion

        /// <summary>
        /// Enable indentation
        /// </summary>
        public bool EnableIndentation { get; set; }

        /// <summary>
        /// BindingFlags for reflection finding properties
        /// </summary>
        public BindingFlags BindingFlags
        {
            get
            {
                if (bindingFlags.HasValue)
                    return bindingFlags.Value;
                return DEFAULT_BINDINGFLAGS;
            }
            set
            {
                bindingFlags = value;
            }
        }
        private BindingFlags? bindingFlags = null;

        /// <summary>
        /// Depth of reflection
        /// </summary>
        public UInt16 Depth
        {
            get
            {
                if (depth.HasValue)
                    return depth.Value;
                return DEFAULT_DEPTH;
            }
            set
            {
                depth = value;
            }
        }
        private UInt16? depth = null;

        private UInt16 CurrentDepth;

        private Type[] simpleType = new Type[] {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(Single)
        };

        /// <summary>
        /// Convert instance of class to string
        /// </summary>
        /// <typeparam name="T">Type of instance</typeparam>
        /// <param name="instance">Instance</param>
        /// <returns>String</returns>
        public string Dump<T>(T instance) where T : class
        {
            if (instance == null)
                return string.Empty;

            CurrentDepth = Depth;

            var sb = new StringBuilder();
            ToString(instance, sb);
            return sb.Replace(CR + CR, "").ToString();
        }

        private void ToString<T>(T instance, StringBuilder sb) where T : class
        {
            if (instance == null)
                return;

            sb.Append(instance.GetType().Name);
            sb.Append(EQUAL);
            ToStringWithoutName(instance, sb);
        }

        private void ToStringWithoutName(object instance, StringBuilder sb)
        {
            if (CurrentDepth == 0)
            {
                sb.Append(OUT_OF_DEPTH);
                return;
            }

            CurrentDepth--;

            sb.Append(PREFIX_OBJECT);
            AddCarriageReturn(sb);

            var isFirst = true;
            foreach (var pi in GetPropperties(instance.GetType()))
            {
                if (!isFirst)
                {
                    sb.Append(DELIMITER);
                    AddCarriageReturn(sb);
                }
                isFirst = false;

                AddIndentation(sb);

                sb.Append(PREFIX_OBJECT);

                sb.Append(pi.Name);
                sb.Append(NAME_VALUE_DELIMITER);

                var value = pi.GetValue(instance, null);
                PropertyToString(value, sb);

                sb.Append(SUFIX_OBJECT);
            }

            CurrentDepth++;

            AddCarriageReturn(sb);
            AddIndentation(sb);
            sb.Append(SUFIX_OBJECT);
        }

        private void PropertyToString(object instance, StringBuilder sb)
        {
            if (instance == null)
            {
                NullPropertyToSTring(instance, sb);
                return;
            }

            var type = instance.GetType();

            if (IsSimpleType(type))
            {
                SimplePropertyToString(instance, sb);
            }
            else if (IsNullableType(type))
            {
                NullablePropertyToString(instance, sb);
            }
            else if (IsKeyValuePairType(type))
            {
                KeyValuePairToString(instance, sb);
            }
            else if (IsEnumerableType(type))
            {
                EnumerablePropertyToString(instance, sb);
            }
            else if (IsEnumType(type))
            {
                EnumPropertyToString(instance, sb);
            }
            else if (IsClassType(type))
            {
                ClassPropertyToString(instance, sb);
            }
            else
            {
                OtherPropertyToString(instance, sb);
            }
        }

        protected virtual List<PropertyInfo> GetPropperties(Type t)
        {
            return t.GetProperties(BindingFlags).ToList();
        }

        private bool IsNullValue(object instance)
        {
            return instance == null;
        }

        private void NullPropertyToSTring(object instance, StringBuilder sb)
        {
            sb.Append(NULL);
        }

        private bool IsSimpleType(Type t)
        {
            return t.IsPrimitive || simpleType.Contains(t);
        }

        private void SimplePropertyToString(object instance, StringBuilder sb)
        {
            var delimiter = string.Empty;
            if (instance.GetType() == typeof(string))
                delimiter = STRING_DELIMITER;

            sb.Append(delimiter);
            sb.Append(instance.ToString());
            sb.Append(delimiter);
        }

        private bool IsNullableType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private void NullablePropertyToString(object instance, StringBuilder sb)
        {
            Type type = Nullable.GetUnderlyingType(instance.GetType());
            var value = Convert.ChangeType(instance, type, null);
            SimplePropertyToString(value, sb);
        }

        private bool IsKeyValuePairType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(KeyValuePair<,>));
        }

        private void KeyValuePairToString(object instance, StringBuilder sb)
        {
            var type = instance.GetType();

            var key = type.GetProperty("Key").GetValue(instance, null);
            var value = type.GetProperty("Value").GetValue(instance, null);

            sb.Append(PREFIX_OBJECT);
            PropertyToString(key, sb);
            sb.Append(NAME_VALUE_DELIMITER);
            PropertyToString(value, sb);
            sb.Append(SUFIX_OBJECT);
        }

        private bool IsEnumerableType(Type t)
        {
            return t.IsArray || t.GetInterfaces().Contains(typeof(IEnumerable));
        }

        private void EnumerablePropertyToString(object instance, StringBuilder sb)
        {
            var isFirst = true;

            sb.Append(PREFIX_ARRAY);
            foreach (var item in (IEnumerable)instance)
            {
                if (!isFirst)
                    sb.Append(DELIMITER);
                isFirst = false;

                PropertyToString(item, sb);
            }
            sb.Append(SUFIX_ARRAY);
        }

        private bool IsEnumType(Type t)
        {
            return t.IsEnum;
        }

        private void EnumPropertyToString(object instance, StringBuilder sb)
        {
            sb.AppendFormat("{0}({1})", instance.ToString(), (int)instance);
        }

        private bool IsClassType(Type type)
        {
            return type.IsClass;
        }

        private void ClassPropertyToString(object instance, StringBuilder sb)
        {
            ToStringWithoutName(instance, sb);
        }

        private void OtherPropertyToString(object instance, StringBuilder sb)
        {
            sb.Append(UNKNOW);
        }

        #region Indentation

        private void AddIndentation(StringBuilder sb)
        {
            if (EnableIndentation)
                for (int i = 0; i < (Depth - CurrentDepth); i++)
                    sb.Append(INDENT);
        }

        private void AddCarriageReturn(StringBuilder sb)
        {
            if (EnableIndentation)
                sb.Append(CR);
        }

        #endregion
    }
}
