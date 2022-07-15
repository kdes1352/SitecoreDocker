using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace AllinaHealth.Framework.Extensions
{
    /// <summary>
    /// Extension methods for generic-based classes
    /// </summary>
    public static class GenericsExtensions
    {
        /// <summary>
        /// Converts a list of ints to a comma-separated list
        /// </summary>
        /// <param name="l">List of ints</param>
        /// <returns>Comma-separated list representation</returns>
        public static string ListToString(this List<int> l)
        {
            return l.ListToString(',');
        }

        /// <summary>
        /// Converts a list of ints to a delimited list
        /// </summary>
        /// <param name="l">List of ints</param>
        /// <param name="delimiter">Delimiter for the list</param>
        /// <returns>Delimited list representation</returns>
        public static string ListToString(this List<int> l, char delimiter)
        {
            StringBuilder s = new StringBuilder();
            if (l != null && l.Count > 0)
                foreach (int i in l)
                {
                    s.AppendFormat("{0}{1}", i, delimiter);
                }

            return s.ToString().TrimEnd(delimiter);
        }

        public static string ListToString(this List<long> l, char delimiter)
        {
            StringBuilder s = new StringBuilder();
            if (l != null && l.Count > 0)
                foreach (int i in l)
                {
                    s.AppendFormat("{0}{1}", i, delimiter);
                }

            return s.ToString().TrimEnd(delimiter);
        }

        /// <summary>
        /// Converts a hashset of ints to a comma-separated list
        /// </summary>
        /// <param name="h">Hashset of ints</param>
        /// <returns>Comma-separated list representation</returns>
        public static string HashsetToString(this HashSet<int> h)
        {
            return h.HashsetToString(',');
        }

        /// <summary>
        /// Converts a hashset of ints to a delimited list
        /// </summary>
        /// <param name="h">Hashset of ints</param>
        /// <param name="delimiter">Delimiter for the list</param>
        /// <returns>Delimited list representation</returns>
        public static string HashsetToString(this HashSet<int> h, char delimiter)
        {
            StringBuilder s = new StringBuilder();
            if (h != null && h.Count > 0)
                foreach (int i in h)
                    s.AppendFormat("{0},", i);
            return s.ToString().TrimEnd(delimiter);
        }

        /// <summary>
        /// Converts a list of strings to a comma-separated list
        /// </summary>
        /// <param name="l">List of strings</param>
        /// <returns>Comma-separated list representation</returns>
        public static string ListToString(this List<string> l)
        {
            return l.ListToString(',');
        }

        /// <summary>
        /// Converts a list of strings to a quoted comma-separated list
        /// </summary>
        /// <param name="l">List of strings</param>
        /// <returns>Quoted comma-separated list representation</returns>
        public static string ListToQuotedString(this List<string> l)
        {
            if (l == null || l.Count < 1)
                return "''";
            StringBuilder sb = new StringBuilder();
            foreach (string s in l)
                sb.AppendFormat("'{0}',", s);

            return sb.ToString(0, sb.Length - 1);
        }

        /// <summary>
        /// Converts a list of strings to a quoted comma-separated list
        /// </summary>
        /// <param name="l">List of strings</param>
        /// <returns>Quoted comma-separated list representation</returns>
        public static string ListToDoubleQuotedString(this List<string> l)
        {
            if (l == null || l.Count < 1)
                return "''";
            StringBuilder sb = new StringBuilder();
            foreach (string s in l)
                sb.AppendFormat("\"{0}\",", s);

            return sb.ToString(0, sb.Length - 1);
        }

        /// <summary>
        /// Converts a list of strings to a delimited list
        /// </summary>
        /// <param name="l">List of string</param>
        /// <param name="delimiter">Delimiter to use</param>
        /// <returns>Delimited list representation</returns>
        public static string ListToString(this List<string> l, char delimiter)
        {
            return (l != null && l.Count > 0 ? string.Join(new string(new[] { delimiter }), l.Where(x => !x.IsNullOrEmptyTrimmed()).ToArray()) : "");
        }

        /// <summary>
        /// Converts a hashset of ints to a comma-separated list
        /// </summary>
        /// <param name="h">Hashset of ints</param>
        /// <returns>Comma-separated list representation</returns>
        public static string HashsetToString(this HashSet<string> h)
        {
            return h.HashsetToString(',');
        }

        /// <summary>
        /// Converts a hashset of ints to a delimited list
        /// </summary>
        /// <param name="h">Hashset of ints</param>
        /// <param name="delimiter">Delimiter for the list</param>
        /// <returns>Delimited list representation</returns>
        public static string HashsetToString(this HashSet<string> h, char delimiter)
        {
            StringBuilder s = new StringBuilder();
            if (h != null && h.Count > 0)
                foreach (string st in h)
                    s.AppendFormat("{0},", st);
            return s.ToString().TrimEnd(delimiter);
        }

        /// <summary>
        /// Converts a list of strings to a delimited list
        /// </summary>
        /// <param name="l">List of string</param>
        /// <param name="delimiter">Delimiter to use</param>
        /// <returns>Delimited list representation</returns>
        public static string ListToString(this List<string> l, string delimiter)
        {
            return (l != null && l.Count > 0 ? string.Join(new string(delimiter.ToCharArray()), l.Where(x => !x.IsNullOrEmptyTrimmed()).ToArray()) : "");
        }

        public static string ListToString(this List<KeyValuePair<string, string>> l)
        {
            if (l == null || l.Count < 1)
                return "";
            StringBuilder s = new StringBuilder();
            foreach (KeyValuePair<string, string> i in l)
            {
                if (s.Length > 0)
                    s.Append("|");

                s.Append(i.Key.Trim() + "^" + i.Value.Trim());
            }
            return s.ToString();
        }

        public static string ListToString(this List<Tuple<string, string, string>> l)
        {
            if (l == null || l.Count < 1)
                return "";
            StringBuilder s = new StringBuilder();
            foreach (Tuple<string, string, string> i in l)
            {
                if (s.Length > 0)
                    s.Append("|");

                s.Append(i.Item1.Trim() + "^" + i.Item2.Trim() + "^" + i.Item3.Trim());
            }
            return s.ToString();
        }

        /// <summary>
        /// Converts a list of KeyValuePairs to the format required for the listbuilders
        /// 
        /// This assumes that the KeyValuePair is in the form of id, text
        /// </summary>
        /// <param name="l">list of KeyValuePairs</param>
        /// <param name="field">field name</param>
        /// <returns>Formatted HTML representation</returns>
        public static string ListToDisplayString(this List<KeyValuePair<int, string>> l, string field)
        {
            if (l != null && l.Count > 0)
            {
                StringBuilder sb = new StringBuilder("<div class=\"lb_hidden\">");
                for (int i = 1; i < l.Count + 1; i++)
                {
                    KeyValuePair<int, string> kvp = l[i - 1];
                    sb.AppendFormat("<input type=\"hidden\" name=\"{0}.{1}.select\" value=\"{2}\" /><input type=\"hidden\" name=\"{0}.{3}.sid\" value=\"{4}\" />",
                        field, i, kvp.Value, i, kvp.Key);
                }
                sb.Append("</div>");
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Converts a list of KeyValuePairs to the format required for the listbuilders
        /// 
        /// This assumes that the KeyValuePair is in the form of url, text
        /// </summary>
        /// <param name="l">List of KeyValuePairs</param>
        /// <param name="field">field name</param>
        /// <returns>Formatted HTML list</returns>
        public static string ListToDisplayString(this List<KeyValuePair<string, string>> l, string field)
        {
            if (l != null && l.Count > 0)
            {
                StringBuilder sb = new StringBuilder("<div class=\"lb_hidden\">");
                for (int i = 1; i < l.Count + 1; i++)
                {
                    KeyValuePair<string, string> kvp = l[i - 1];
                    sb.AppendFormat("<input type=\"hidden\" name=\"{0}.{1}.item\" value=\"{2}\" /><input type=\"hidden\" name=\"{0}.{3}.url\" value=\"{4}\" />",
                        field, i, kvp.Value, i, kvp.Key);
                }
                sb.Append("</div>");
                return sb.ToString();
            }

            return "";
        }

        /// <summary>
        /// Converts a list of KeyValuePairs to the format required for the listbuilders
        /// 
        /// This assumes that the KeyValuePair is in the form of id, text
        /// </summary>
        /// <param name="l">list of KeyValuePairs</param>
        /// <param name="field">field name</param>
        /// <param name="isSelect"></param>
        /// <returns>Formatted HTML representation</returns>
        public static string ListToDisplayString(this List<KeyValuePair<string, string>> l, string field, bool isSelect)
        {
            if (isSelect)
            {
                if (l != null && l.Count > 0)
                {
                    StringBuilder sb = new StringBuilder("<div class=\"lb_hidden\">");
                    for (int i = 1; i < l.Count + 1; i++)
                    {
                        KeyValuePair<string, string> kvp = l[i - 1];
                        sb.AppendFormat("<input type=\"hidden\" name=\"{0}.{1}.select\" value=\"{2}\" /><input type=\"hidden\" name=\"{0}.{3}.sid\" value=\"{4}\" />",
                            field, i, kvp.Value, i, kvp.Key);
                    }
                    sb.Append("</div>");
                    return sb.ToString();
                }
                return "";
            }
            return ListToDisplayString(l, field);
        }

        /// <summary>
        /// Converts a list of KeyValuePairs to equivalent links for display
        /// 
        /// This assumes that the KeyValuePair is in the form of url, text
        /// </summary>
        /// <param name="l">List of KeyValuePairs</param>
        /// <returns>Formatted HTML links</returns>
        public static string ListToReadOnlyDisplayString(this List<KeyValuePair<string, string>> l)
        {
            if (l != null && l.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> kvp in l)
                {
                    sb.AppendFormat("<a href=\"{0}\" alt=\"{1}\">{1}</a><br/>",
                        kvp.Key, kvp.Value);
                }
                return sb.ToString();
            }
            return "";
        }

        /// <summary>
        /// Converts a list of KeyValuePairs to equivalent links for display
        /// 
        /// This assumes that the KeyValuePair is in the form of id, text
        /// </summary>
        /// <param name="l">List of KeyValuePairs</param>
        /// <param name="isSelect">Is this for a regular select?</param>
        /// <returns>Formatted HTML links</returns>
        public static string ListToReadOnlyDisplayString(this List<KeyValuePair<string, string>> l, bool isSelect)
        {
            if (!isSelect)
                return l.ListToReadOnlyDisplayString();

            if (l != null && l.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> kvp in l)
                {
                    sb.AppendFormat("{0}<br/>", kvp.Value);
                }
                return sb.ToString();
            }
            return "";
        }

        /// <summary>
        /// Converts a list of strings to the format required for the listbuilders
        /// </summary>
        /// <param name="l">List of strings</param>
        /// <param name="field">field name</param>
        /// <returns>Formatted HTML list</returns>
        public static string ListToDisplayString(this List<string> l, string field)
        {
            if (l != null && l.Count > 0)
            {
                StringBuilder sb = new StringBuilder("<div class=\"lb_hidden\">");
                for (int i = 1; i < l.Count + 1; i++)
                {
                    sb.AppendFormat("<input type=\"hidden\" name=\"{0}.{1}.item\" value=\"{2}\" /><input type=\"hidden\" name=\"{0}.{1}.foo\" value=\"{2}\" />",
                        field, i, l[i - 1]);
                }
                sb.Append("</div>");
                return sb.ToString();
            }
            return "";
        }

        public static string ListToDisplayString<T>(this List<T> l, string fieldName, string fieldId, string fieldValue)
        {
            if (l != null && l.Any())
            {
                StringBuilder sb = new StringBuilder("<div class=\"lb_hidden\">");
                int i = 1;
                foreach (var itm in l)
                {
                    if (itm.GetType().GetProperty(fieldValue) == null) { }//.GetValue(itm, null) == null) { }
                    //if (itm.GetType().GetProperty(fieldValue).GetValue(itm, null) == null) { }

                    var test1 = itm.GetType().GetProperty(fieldValue).GetValue(itm, null);
                    var test2 = itm.GetType().GetProperty(fieldId).GetValue(itm, null);

                    sb.AppendFormat("<input type=\"hidden\" name=\"{0}.{1}.select\" value=\"{2}\" /><input type=\"hidden\" name=\"{0}.{3}.sid\" value=\"{4}\" />",
                   fieldName, i, itm.GetType().GetProperty(fieldValue).GetValue(itm, null), i, itm.GetType().GetProperty(fieldId).GetValue(itm, null));

                    i++;
                }
                sb.Append("</div>");
                return sb.ToString();
            }
            return "";
        }


        public static string ListToDisplayStringItemUrl<T>(this List<T> l, string fieldType, string fieldItem, string fieldUrl)
        {
            if (l != null && l.Any())
            {
                StringBuilder sb = new StringBuilder("<div class=\"lb_hidden\">");
                int i = 1;
                foreach (var itm in l)
                {
                    sb.AppendFormat("<input type=\"hidden\" name=\"{0}.{1}.item\" value=\"{2}\" /><input type=\"hidden\" name=\"{0}.{3}.url\" value=\"{4}\" />",
                        fieldType, i, itm.GetType().GetProperty(fieldItem).GetValue(itm, null), i, itm.GetType().GetProperty(fieldUrl).GetValue(itm, null));

                    i++;
                }
                sb.Append("</div>");
                return sb.ToString();
            }
            return "";
        }

        public static string ListToDisplayStringItem<T>(this List<T> l, string itemName, string fieldName)
        {
            if (l != null && l.Any())
            {
                StringBuilder sb = new StringBuilder("<div class=\"lb_hidden\">");
                int i = 1;
                foreach (var itm in l)
                {
                    sb.AppendFormat("<input type=\"hidden\" name=\"{0}.{1}.item\" value=\"{2}\" /><input type=\"hidden\" name=\"{0}.{1}.foo\" value=\"{2}\" />",
                            itemName, i, itm.GetType().GetProperty(fieldName).GetValue(itm, null));

                    i++;
                }
                sb.Append("</div>");
                return sb.ToString();
            }
            return "";
        }

        /// <summary>
        /// Converts a comma-separated string into a list of ints
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <returns>List of ints</returns>
        public static List<int> StringToIntList(this string val)
        {
            return val.StringToIntList(',');
        }

        /// <summary>
        /// Converts a delimited string into a list of ints
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <param name="delimiter">Delimiter</param>
        /// <returns>List of ints</returns>
        public static List<int> StringToIntList(this string val, char delimiter)
        {
            List<int> l = new List<int>();
            if (!val.IsNullOrEmptyTrimmed())
                foreach (string s in val.Split(delimiter))
                {
                    if (!s.CleanNonNumeric().IsNullOrEmptyTrimmed())
                    {
                        l.Add(Convert.ToInt32(s.Trim()));
                    }
                }
            return l;
        }

        public static List<long> StringToLongList(this string val, char delimiter)
        {
            List<long> l = new List<long>();
            if (!val.IsNullOrEmptyTrimmed())
                foreach (string s in val.Split(delimiter))
                {
                    if (!s.CleanNonNumeric().IsNullOrEmptyTrimmed())
                    {
                        l.Add(Convert.ToInt64(s.Trim()));
                    }
                }
            return l;
        }


        public static List<long> StringValuesToList(this string val)
        {
            List<long> l = new List<long>();
            if (!val.IsNullOrEmptyTrimmed())
            {
                foreach (string s in val.Split('|'))
                {
                    if (!s.IsNullOrEmptyTrimmed() && s.IndexOf("^", StringComparison.Ordinal) > -1)
                    {
                        string[] tmp = s.Split('^');
                        l.Add(tmp[0].Trim().ToSLong());
                    }
                }
            }
            return l;
        }



        /// <summary>
        /// Converts a comma-separated string into a list of ints
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <returns>List of ints</returns>
        public static HashSet<int> StringToIntHashset(this string val)
        {
            return val.StringToIntHashset(',');
        }

        /// <summary>
        /// Converts a delimited string into a list of ints
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <param name="delimiter">Delimiter</param>
        /// <returns>List of ints</returns>
        public static HashSet<int> StringToIntHashset(this string val, char delimiter)
        {
            HashSet<int> l = new HashSet<int>();
            if (!val.IsNullOrEmptyTrimmed())
                foreach (string s in val.Split(delimiter))
                {
                    if (!s.CleanNonNumeric().IsNullOrEmptyTrimmed())
                    {
                        l.Add(Convert.ToInt32(s.Trim()));
                    }
                }
            return l;
        }

        /// <summary>
        /// Converts a comma-separated string into a list of strings
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <returns>List of strings</returns>
        public static List<string> StringToStringList(this string val)
        {
            return val.StringToStringList(',');
        }

        /// <summary>
        /// Converts a delimited string into a list of strings
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <param name="delimiter">Delimiter</param>
        /// <returns>List of strings</returns>
        public static List<string> StringToStringList(this string val, char delimiter)
        {
            List<string> l = new List<string>();
            if (!val.IsNullOrEmptyTrimmed())
                foreach (string s in val.Split(delimiter))
                {
                    if (!s.IsNullOrEmptyTrimmed())
                    {
                        l.Add(s.Trim());
                    }
                }
            return l;
        }

        /// <summary>
        /// Converts a comma-separated string into a list of strings
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <returns>List of strings</returns>
        public static HashSet<string> StringToStringHashset(this string val)
        {
            return val.StringToStringHashset(',');
        }

        /// <summary>
        /// Converts a delimited string into a list of strings
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <param name="delimiter">Delimiter</param>
        /// <returns>List of strings</returns>
        public static HashSet<string> StringToStringHashset(this string val, char delimiter)
        {
            HashSet<string> l = new HashSet<string>();
            if (!val.IsNullOrEmptyTrimmed())
                foreach (string s in val.Split(delimiter))
                {
                    if (!s.IsNullOrEmptyTrimmed())
                    {
                        l.Add(s.Trim());
                    }
                }
            return l;
        }

        public static List<KeyValuePair<string, string>> StringToKVPList(this string val)
        {
            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();
            if (!val.IsNullOrEmptyTrimmed())
            {
                foreach (string s in val.Split('|'))
                {
                    if (!s.IsNullOrEmptyTrimmed() && s.IndexOf("^", StringComparison.Ordinal) > -1)
                    {
                        string[] tmp = s.Split('^');
                        l.Add(new KeyValuePair<string, string>(tmp[0].Trim(), tmp[1].Trim()));
                    }
                }
            }
            return l;
        }

        public static List<Tuple<string, string, string>> StringToTupleList(this string val)
        {
            List<Tuple<string, string, string>> l = new List<Tuple<string, string, string>>();
            if (!val.IsNullOrEmptyTrimmed())
            {
                foreach (string s in val.Split('|'))
                {
                    if (!s.IsNullOrEmptyTrimmed() && s.IndexOf("^", StringComparison.Ordinal) > -1)
                    {
                        string[] tmp = s.Split('^');
                        if (tmp.Length == 3)
                            l.Add(new Tuple<string, string, string>(tmp[0].Trim(), tmp[1].Trim(), tmp[2].Trim()));
                    }
                }
            }
            return l;
        }

        /// <summary>
        /// Takes in a list and returns just the unique items
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="l">List to make unique</param>
        /// <returns>Unique List</returns>
        public static List<T> UniqueList<T>(this List<T> l)
        {
            List<T> retval = new List<T>();
            if (l != null && l.Count > 0)
                foreach (T i in l)
                {
                    if (!retval.Contains(i))
                        retval.Add(i);
                }
            return retval;
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        /// <summary>
        /// Takes in a list and returns just the unique items
        /// </summary>
        /// <param name="l">List to make unique</param>
        /// <param name="insensitive">Case insensitive?</param>
        /// <returns>Unique List</returns>
        public static List<string> UniqueList(this List<string> l, bool insensitive)
        {
            List<string> retval = new List<string>();
            if (l != null && l.Count > 0)
            {
                if (insensitive)
                {
                    List<string> keys = new List<string>();
                    foreach (string i in l)
                    {
                        string lower = i.ToLower();
                        if (!keys.Contains(lower))
                        {
                            keys.Add(lower);
                            retval.Add(i);
                        }
                    }
                }
                else
                {
                    foreach (string i in l)
                        if (!retval.Contains(i))
                            retval.Add(i);
                }
            }
            return retval;
        }

        /// <summary>
        /// Takes in a list and returns just the unique items
        /// </summary>
        /// <param name="l">List to make unique</param>
        /// <param name="insensitive">Case insensitive?</param>
        /// <returns>Unique List</returns>
        public static List<KeyValuePair<string, string>> UniqueList(this List<KeyValuePair<string, string>> l, bool insensitive)
        {
            List<KeyValuePair<string, string>> retval = new List<KeyValuePair<string, string>>();
            if (l != null && l.Count > 0)
            {
                if (insensitive)
                {
                    List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string, string>>();
                    foreach (KeyValuePair<string, string> kvp in l)
                    {
                        KeyValuePair<string, string> tmp = new KeyValuePair<string, string>(kvp.Key.ToLower(), kvp.Value.ToLower());
                        if (!keys.Contains(tmp))
                        {
                            keys.Add(tmp);
                            retval.Add(kvp);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, string> kvp in l)
                        if (!retval.Contains(kvp))
                            retval.Add(kvp);
                }
            }
            return retval;
        }

        /// <summary>
        /// Takes in an array of KeyValuePairs and returns a list of SelectListItem
        /// </summary>
        /// <typeparam name="T1">First type</typeparam>
        /// <typeparam name="T2">Second type</typeparam>
        /// <param name="col">List of KeyValuePairs</param>
        /// <param name="selectedItem">Item to pre-select</param>
        /// <returns>List of SelectListItems</returns>
        public static IEnumerable<SelectListItem> ToSelectListItems<T1, T2>(this KeyValuePair<T1, T2>[] col, string selectedItem)
        {
            return col.AsEnumerable().ToSelectListItems(selectedItem);
        }

        /// <summary>
        /// Takes in an array of KeyValuePairs and returns a list of SelectListItem
        /// </summary>
        /// <typeparam name="T1">First type</typeparam>
        /// <typeparam name="T2">Second type</typeparam>
        /// <param name="col">List of KeyValuePairs</param>
        /// <param name="selectedItem">Item to pre-select</param>
        /// <returns>List of SelectListItems</returns>
        public static IEnumerable<SelectListItem> ToSelectListItemsNoReorder<T1, T2>(this KeyValuePair<T1, T2>[] col, string selectedItem)
        {
            return col.AsEnumerable().ToSelectListItemsNoReorder(selectedItem);
        }

        /// <summary>
        /// Takes in IEnumerable of KeyValuePairs and returns a list of SelectListItem
        /// </summary>
        /// <typeparam name="T1">First type</typeparam>
        /// <typeparam name="T2">Second type</typeparam>
        /// <param name="col">List of KeyValuePairs</param>
        /// <param name="selectedItem">Item to pre-select</param>
        /// <returns>List of SelectListItems</returns>
        public static IEnumerable<SelectListItem> ToSelectListItems<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> col, string selectedItem)
        {
            return col.OrderBy(item => item.Value)
                      .Select(item => new SelectListItem
                      {
                          Selected = (selectedItem != null && selectedItem.Equals(item.Key.ToString(), StringComparison.CurrentCultureIgnoreCase)),
                          Text = item.Value.ToString(),
                          Value = item.Key.ToString()
                      });
        }

        /// <summary>
        /// Takes in IEnumerable of KeyValuePairs and returns a list of SelectListItem
        /// </summary>
        /// <typeparam name="T1">First type</typeparam>
        /// <typeparam name="T2">Second type</typeparam>
        /// <param name="col">List of KeyValuePairs</param>
        /// <param name="selectedItem">Item to pre-select</param>
        /// <returns>List of SelectListItems</returns>
        public static IEnumerable<SelectListItem> ToSelectListItemsNoReorder<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> col, string selectedItem)
        {
            return col.Select(item => new SelectListItem
            {
                Selected = (selectedItem != null && selectedItem.Equals(item.Key.ToString(), StringComparison.CurrentCultureIgnoreCase)),
                Text = item.Value.ToString(),
                Value = item.Key.ToString()
            });
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");

            var values = from TEnum e in Enum.GetValues(typeof(TEnum)) select new { ID = (int)Enum.Parse(typeof(TEnum), e.ToString()), Name = e.ToString() };
            //var values = from TEnum e in Enum.GetValues(typeof(TEnum)) select new { ID = e, Name = e.ToString() };

            return new SelectList(values, "ID", "Name", enumObj);
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj, string selectedValue) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");

            var values = from TEnum e in Enum.GetValues(typeof(TEnum)) select new { ID = (int)Enum.Parse(typeof(TEnum), e.ToString()), Name = e.ToString() };
            //var values = from TEnum e in Enum.GetValues(typeof(TEnum)) select new { ID = e, Name = e.ToString() };
            if (string.IsNullOrWhiteSpace(selectedValue))
            {
                return new SelectList(values, "ID", "Name", enumObj);
            }
            return new SelectList(values, "ID", "Name", selectedValue);
        }


        /// <summary>
        /// Pages through an IEnumerable
        /// 
        /// from http://solidcoding.blogspot.com/2007/11/paging-with-linq.html
        /// </summary>
        /// <typeparam name="TSource">Type of the source</typeparam>
        /// <param name="source">Source queryable</param>
        /// <param name="page">Page to go to</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged list</returns>
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            if (source == null)
                return null;
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IEnumerable<TSource> Page<TSource>(this List<TSource> list, int page, int pageSize)
        {
            if (list == null)
                return null;

            return list.Skip((page - 1) * pageSize).Take(pageSize);
        }


        /// <summary>
        /// Converts the given list to a string, suitable for the database
        /// </summary>
        /// <param name="l">List to convert</param>
        /// <returns>String representation</returns>
        public static string ListToString(this List<Uri> l)
        {
            if (l == null || l.Count < 1)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (Uri u in l)
            {
                sb.AppendFormat("{0}|", u);
            }
            return sb.ToString().TrimEnd('|');
        }

        /// <summary>
        /// Converts the given string to a list of uris
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <returns>Corresponding list</returns>
        public static List<Uri> StringToUriList(this string val)
        {
            List<Uri> l = new List<Uri>();
            if (!val.IsNullOrEmptyTrimmed())
                foreach (string s in val.Split('|'))
                    if (!s.IsNullOrEmptyTrimmed())
                        l.Add(new Uri(s));

            return l;
        }

        /// <summary>
        /// Converts a string array to a comma-delimited string
        /// </summary>
        /// <param name="arr">String array</param>
        /// <returns>Delimited string</returns>
        public static string StringArrayToString(this string[] arr)
        {
            if (arr == null || arr.Length < 1) return string.Empty;

            string sArr = string.Empty;
            foreach (string s in arr)
            {
                sArr += string.Format("{0},", s);
            }
            return sArr.TrimEnd(',');
        }


    }
}
