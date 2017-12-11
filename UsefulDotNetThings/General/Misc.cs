using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UsefulDotNetThings.General
{
    public static class Misc
    {
        /// <summary>
        /// Creates string representation of object in format:
        /// --- CLASS NAME ---
        /// Property = value
        /// ...
        /// --- END CLASS NAME ---
        /// </summary>
        /// <param name="obj">Object to get property description of.</param>
        /// <param name="level">Used for recursion, and indicates the depth of the current class in property tree.</param>
        /// <param name="propName">Name of current property being stringified.</param>
        /// <returns>String of object.</returns>
        public static string StringifyObject(object obj, int level = 0, string propName = null)
        {
            var propertyList = TypeDescriptor.GetProperties(obj);
            StringBuilder sb = new StringBuilder();
            var classname = TypeDescriptor.GetClassName(obj);
            string tags = new string(Enumerable.Repeat('-', level * 3).ToArray());
            string spacing = new string(Enumerable.Repeat(' ', level * 3).ToArray());

            if (propertyList.Count == 0)
                return spacing + $"{propName} = {obj}";

            sb.AppendLine($"{tags} {classname} {tags}");
            foreach (PropertyDescriptor descriptor in propertyList)
                sb.AppendLine(spacing + StringifyObject(descriptor.GetValue(obj), level + 1, descriptor.Name));

            sb.AppendLine($"{tags} END {classname} {tags}");


            return sb.ToString();
        }

        /// <summary>
        /// Gets Descriptions on Enum members.
        /// </summary>
        /// <param name="theEnum">Enum to get descriptions from.</param>
        /// <returns>Description of enum member.</returns>
        public static string GetEnumDescription(Enum theEnum)
        {
            if (theEnum == null)
                return null;

            FieldInfo info = theEnum.GetType().GetField(theEnum.ToString());
            object[] attribs = info.GetCustomAttributes(false);
            if (attribs.Length == 0)
                return theEnum.ToString();
            else
                return (attribs[0] as DescriptionAttribute)?.Description;
        }
    }
}
