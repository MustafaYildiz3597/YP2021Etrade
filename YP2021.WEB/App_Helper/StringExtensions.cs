using System;
using System.ComponentModel;

namespace Nero2021
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Removes dashes ("-") from the given object value represented as a string and returns an empty string ("")
        ///     when the instance type could not be represented as a string.
        ///     <para>
        ///         Note: This will return the type name of given isntance if the runtime type of the given isntance is not a
        ///         string!
        ///     </para>
        /// </summary>
        /// <param name="value">The object instance to undash when represented as its string value.</param>
        /// <returns></returns>
        public static string UnDash(this object value)
        {
            return ((value as string) ?? string.Empty).UnDash();
        }

        /// <summary>
        ///     Removes dashes ("-") from the given string value.
        /// </summary>
        /// <param name="value">The string value that optionally contains dashes.</param>
        /// <returns></returns>
        public static string UnDash(this string value)
        {
            return (value ?? string.Empty).Replace("-", string.Empty);
        }

        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static string DescriptionAttr<T>(this T source)
        {
            System.Reflection.FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }

        #region sort functions
        public static int SortString(string s1, string s2, string sortDirection)
        {
            return sortDirection == "asc" ? (s1 ?? "").CompareTo(s2 ?? "") : (s2 ?? "").CompareTo(s1 ?? "");
        }
        public static int SortInteger(string s1, string s2, string sortDirection)
        {
            int i1 = string.IsNullOrEmpty(s1) ? int.MinValue : int.Parse(s1);
            int i2 = string.IsNullOrEmpty(s2) ? int.MinValue : int.Parse(s2);
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }
        public static int SortDecimal(string s1, string s2, string sortDirection)
        {
            Decimal d1 = string.IsNullOrEmpty(s1) ? Decimal.MinValue : Decimal.Parse(s1);
            Decimal d2 = string.IsNullOrEmpty(s2) ? Decimal.MinValue : Decimal.Parse(s2);
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }
        public static int SortDateTime(string s1, string s2, string sortDirection)
        {
            DateTime d1 = string.IsNullOrEmpty(s1) ? DateTime.MinValue : DateTime.Parse(s1);
            DateTime d2 = string.IsNullOrEmpty(s2) ? DateTime.MinValue : DateTime.Parse(s2);
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }
        #endregion
    }
}