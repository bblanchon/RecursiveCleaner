using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Linq.Expressions;
using System.Globalization;

namespace RecursiveCleaner.Config
{
    class AttributeParser
    {
        class AttributeInfo
        {
            public string Name;
            public string Value;
            public bool HasBeenUsed;
        }

        readonly Dictionary<string, AttributeInfo> dict;

        public AttributeParser(XmlReader xml)
        {
            var element = "<" + xml.Name + ">";
            dict = new Dictionary<string, AttributeInfo>(xml.AttributeCount);

            if (xml.MoveToFirstAttribute())
            {
                do
                {
                    dict.Add(xml.Name.ToLower(), new AttributeInfo
                    {
                        Name = xml.Name,
                        Value = xml.Value
                    });

                } while (xml.MoveToNextAttribute());
            }
        }

        private AttributeInfo GetAttribute(string name)
        {
            var key = name.ToLower();
            return dict.ContainsKey(key) ? dict[key] : null;
        }

        public int Count
        {
            get
            {
                return dict.Count;
            }
        }

        public bool HasUnused
        {
            get { return dict.Values.Any(x => !x.HasBeenUsed); }
        }

        public IEnumerable<string> Unused
        {
            get
            {
                return dict.Values.Where(x => !x.HasBeenUsed).Select(x=>x.Name);
            }
        }

        public void AssertNoUnused()
        {
            if (HasUnused) throw new UnknownAttributesException(Unused);
        }

        public void Get<T>(string attributeName, Expression<Func<T>> property, bool isRequired = false)
        {
            var attr = GetAttribute(attributeName);

            if (attr == null)
            {
                if (isRequired) throw new Exception("Attribute " + attributeName + " is missing");
                else return;
            }                 
       
            T value;
            if (TryParse(attr.Value, out value))
            {
                var assign = Expression.Assign(property.Body, Expression.Constant(value));
                Expression.Lambda(assign).Compile().DynamicInvoke();
            }
            else
            {
                throw new FormatException(string.Format(
                    "\"{0}\" is not a valid value for attribute {1}",
                    attr.Value, attributeName));
            }

            attr.HasBeenUsed = true;
        }

        #region Private parsing helpers

        private static bool TryParse<T>(string input, out T value)
        {
            try
            {
                if (typeof(T).IsEnum)
                    value = (T)Enum.Parse(typeof(T), input, true);
                else if (typeof(T) == typeof(bool))
                    value = (T)(object)ParseBool(input);
                else
                    value = (T)Convert.ChangeType(input, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception)
            {
                value = default(T);
                return false;
            }
        }

        private static bool ParseBool(string input)
        {
            switch (input.Trim().ToLower())
            {
                case "yes":
                case "true":
                case "1":
                    return true;

                case "no":
                case "false":
                case "0":
                    return false;

                default:
                    throw new InvalidCastException(string.Format("\"{0}\" can't be converted to a boolean", input));
            }
        }

        #endregion
    }
}
