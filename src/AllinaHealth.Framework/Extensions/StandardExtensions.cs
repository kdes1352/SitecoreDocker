using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Newtonsoft.Json;

namespace AllinaHealth.Framework.Extensions
{
    /// <summary>
    /// Extension methods for standard classes
    /// </summary>
    public static class StandardExtensions
    {
        /// <summary>
        /// Returns the last (x) characters from a string
        /// </summary>
        /// <param name="val">String to use</param>
        /// <param name="length">Length of the returned value</param>
        /// <returns>Last (x) characters</returns>
        public static string GetLast(this string val, int length)
        {
            length = Math.Max(length, 0);

            return val.Length > length ? val.Substring(val.Length - length, length) : val;
        }

        /// <summary>
        /// Takes out all non-numeric characters and returns a string containing only numbers
        /// </summary>
        /// <param name="val">String to clean</param>
        /// <returns>Numeric-only string</returns>
        public static string CleanNonNumeric(this string val)
        {
            return val.IsNullOrEmptyTrimmed() ? string.Empty : Regex.Replace(val, "[^0-9]", "");
        }

        /// <summary>
        /// Takes out all non-numeric (and non-'x') characters and returns a string containing only numbers and 'x'
        /// </summary>
        /// <param name="val">String to clean</param>
        /// <returns>Numeric-only string</returns>
        public static string CleanPhone(this string val)
        {
            return Regex.Replace(val, "[^0-9Xx]", "");
        }

        /// <summary>
        /// Takes out all non-numeric characters and returns a string containing only numbers
        /// </summary>
        /// <param name="val">String to clean</param>
        /// <param name="allowNegative">Allow negative numbers?</param>
        /// <returns>Numeric-only string</returns>
        public static string CleanNonNumeric(this string val, bool allowNegative)
        {
            return allowNegative ? Regex.Replace(val, @"[^\d-]", "") : val.CleanNonNumeric();
        }


        public static int DateDiffDays(DateTime dt1, DateTime dt2)
        {
            return (dt1 - dt2).Days;
        }

        public static int DateDiffYears(DateTime dt1, DateTime dt2)
        {
            return (dt2.Year - dt1.Year);
        }

        /// <summary>
        /// Converts the given object to an int
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent int</returns>
        public static int ToSInt(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return 0;

            return int.TryParse(val.ToSStr(), out var res) ? res : 0;
        }

        /// <summary>
        /// Converts the given object to a long
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent long</returns>
        public static long ToSLong(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return 0;

            return long.TryParse(val.ToSStr(), out var res) ? res : 0;
        }

        /// <summary>
        /// Converts the given object to a nullable int
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent nullable int</returns>
        // ReSharper disable once InconsistentNaming
        public static int? ToSNInt(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return null;

            if (int.TryParse(val.ToSStr(), out var res))
            {
                return res;
            }

            return null;
        }

        /// <summary>
        /// Converts the given object to a nullable long
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent nullable long</returns>
        // ReSharper disable once InconsistentNaming
        public static long? ToSNLong(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return null;

            if (long.TryParse(val.ToSStr(), out var res))
            {
                return res;
            }

            return null;
        }

        /// <summary>
        /// Converts the given object to a nullable short
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent nullable short</returns>
        // ReSharper disable once InconsistentNaming
        public static short? ToSNShort(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return null;

            if (short.TryParse(val.ToSStr(), out var res))
            {
                return res;
            }

            return null;
        }

        /// <summary>
        /// Converts the given object to a string
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent string</returns>
        public static string ToSStr(this object val)
        {
            if (val == null || Convert.IsDBNull(val))
                return "";

            var res = Convert.ToString(val);
            return (res.Equals("null", StringComparison.CurrentCultureIgnoreCase) ? "" : res.Trim());
        }

        /// <summary>
        /// Converts the given object to a double
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent double</returns>
        public static double ToSDbl(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return 0d;

            return double.TryParse(val.ToSStr(), out var res) ? res : 0d;
        }

        /// <summary>
        /// Converts the given object to a decimal
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent decimal</returns>
        public static decimal ToSDecimal(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return 0.0M;

            return decimal.TryParse(val.ToSStr(), out var res) ? res : 0.0M;
        }

        /// <summary>
        /// Converts the given object to a nullable double
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent nullable double</returns>
        // ReSharper disable once InconsistentNaming
        public static double? ToSNDbl(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return null;

            if (double.TryParse(val.ToSStr(), out var res))
                return res;

            return null;
        }

        /// <summary>
        /// Converts the given object to a nullable decimal
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent nullable decimal</returns>
        // ReSharper disable once InconsistentNaming
        public static decimal? ToSNDecimal(object val)
        {
            switch (val)
            {
                case DBNull _:
                case null:
                case string s when s.Length == 0:
                    return null;
                default:
                    return Convert.ToDecimal(val);
            }
        }

        /// <summary>
        /// Converts the given object to a byte array
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent byte array</returns>
        public static byte[] ToSByteArray(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return null;

            try
            {
                return (byte[])val;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the given object to a boolean
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent boolean</returns>
        public static bool ToSBool(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "" || (val as string) == "0")
                return false;

            string s = val.ToSStr();
            if (s.Equals("1"))
                return true;

            if (s.Equals("True", StringComparison.CurrentCultureIgnoreCase))
                return true;

            if (s.ToUpper().Equals("TRUE"))
                return true;

            if (s.Equals("0"))
                return false;

            if (s.Equals("False", StringComparison.CurrentCultureIgnoreCase))
                return false;

            if (s.ToUpper().Equals("FALSE"))
                return false;

            bool res;
            if (bool.TryParse(s, out res))
                return res;

            return false;
        }

        public static bool ToSBool(this object val, bool defaultVal)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "" || (val as string) == "0")
                return defaultVal;

            return ToSBool(val);
        }

        /// <summary>
        /// Converts the given object to a nullable boolean
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent nullable boolean</returns>
        public static bool? ToSNBool(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "" || (val as string) == "0")
                return null;

            string s = val.ToSStr();
            if (s.Equals("1"))
                return true;

            if (s.Equals("True"))
                return true;

            if (s.Equals("0"))
                return false;

            if (s.Equals("False"))
                return false;

            if (s.Equals(""))
                return null;

            bool res;
            if (bool.TryParse(s, out res))
                return res;

            return null;
        }

        /// <summary>
        /// Converts the given object to a byte
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent byte</returns>
        public static byte ToSByte(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return default(byte);

            byte res;
            if (byte.TryParse(val.ToSStr(), out res))
                return res;

            return default(byte);
        }

        /// <summary>
        /// Converts the given object to a nullable datetime
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent DateTime</returns>
        public static DateTime? ToSDate(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "" || (val as string) == "0/0/0")
                return null;
            DateTime res;
            if (DateTime.TryParse(val.ToSStr(), out res))
                return res;

            return null;
        }

        public static DateTime? ToSDateLong(this long val)
        {
            if (val <= 0)
                return null;

            string s = val.ToSStr();
            DateTime res;
            if (DateTime.TryParse(s.Substring(4, 2) + "/" + s.Substring(6, 2) + "/" + s.Substring(0, 4), out res))
                return res;

            return null;
        }

        public static DateTime? ToSDate(this object val, DateTime defaultDateTime)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "" || (val as string) == "0/0/0")
                return defaultDateTime;

            DateTime res;
            if (DateTime.TryParse(val.ToSStr(), out res))
                return res;

            return defaultDateTime;
        }

        public static DateTime ToDate(this object val, DateTime defaultDateTime)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "" || (val as string) == "0/0/0")
                return defaultDateTime;

            DateTime res;
            if (DateTime.TryParse(val.ToSStr(), out res))
                return res;

            return defaultDateTime;
        }

        /// <summary>
        /// Converts the given object to a nullable datetime
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent DateTime</returns>
        public static long ToSLongDate(this DateTime val)
        {
            try
            {
                DateTime res;
                if (DateTime.TryParse(val.ToSStr(), out res))
                {
                    return res.ToString("yyyyMMdd").ToSLong();
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return 0;
        }

        public static long ToSLongDateTime(this DateTime val)
        {
            try
            {
                DateTime res;
                if (DateTime.TryParse(val.ToSStr(), out res))
                {
                    return res.ToString("yyyyMMddHHmmss").ToSLong();
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// Converts the given object to a timespan
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent TimeSpan</returns>
        public static TimeSpan ToSTime(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return TimeSpan.Zero;

            if (val is TimeSpan)
                return (TimeSpan)val;

            DateTime? dt = val.ToSDate();
            return dt == null ? default(TimeSpan) : ((DateTime)dt).TimeOfDay;
        }

        /// <summary>
        /// Converts the given object to a nullable timespan
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent nullable TimeSpan</returns>
        public static TimeSpan? ToSNTime(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return null;

            if (val is TimeSpan)
                return (TimeSpan)val;

            string tmp = val.ToSStr().Replace(".", "");
            DateTime? dt = (tmp.Equals("noon", StringComparison.CurrentCultureIgnoreCase) ? "12:00pm" :
                tmp.Equals("midnight", StringComparison.CurrentCultureIgnoreCase) ? "12:00am" :
                tmp).ToSDate();

            return dt == null ? null : (TimeSpan?)((DateTime)dt).TimeOfDay;
        }

        /// <summary>
        /// Converts the given timespan to a formatted string
        /// </summary>
        /// <param name="val">TimeSpan to convert</param>
        /// <returns>Equivalent string</returns>
        public static string ToTimeString(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return "";
            if (val is TimeSpan)
                return (DateTime.Today + (TimeSpan)val).ToString("h:mm tt");

            DateTime? dt = val.ToSDate();
            return dt == null ? "" : ((DateTime)dt).ToString("h:mm tt");
        }

        /// <summary>
        /// Converts the given timespan to a formatted string
        /// </summary>
        /// <param name="val">TimeSpan to convert</param>
        /// <returns>Equivalent string</returns>
        public static string ToAllinaStyledTimeString(this object val)
        {
            return val.ToTimeString().Replace(":00", "").Replace("AM", "a.m.").Replace("PM", "p.m.");
        }

        /// <summary>
        /// Converts the given day of the week to a formatted string
        /// </summary>
        /// <param name="val">Day of week to convert</param>
        /// <returns>Equivalent string</returns>
        public static string ToAllinaStyledDayofWeekString(this object val)
        {
            return val.ToSStr().Replace("Monday", "<span aria-labelledby=\"Monday\">Mon</span>").Replace("Tuesday", "<span aria-labelledby=\"Tuesday\">Tue</span>").Replace("Wednesday", "<span aria-labelledby=\"Wednesday\">Wed</span>").Replace("Thursday", "<span aria-labelledby=\"Thursday\">Thu</span>").Replace("Friday", "<span aria-labelledby=\"Friday\">Fri</span>").Replace("Saturday", "<span aria-labelledby=\"Saturday\">Sat</span>").Replace("Sunday", "<span aria-labelledby=\"Sunday\">Sun</span>");
        }

        /// <summary>
        /// Converts the given timespan to a formatted string
        /// </summary>
        /// <param name="val">TimeSpan to convert</param>
        /// <returns>Equivalent string</returns>
        public static string ToDateString(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return "";

            DateTime? dt = val.ToSDate();
            return dt == null ? "" : ((DateTime)dt).ToString("M/d/yyyy");
        }

        /// <summary>
        /// Converts the given object into a date/time string
        /// </summary>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent string</returns>
        public static string ToDateTimeString(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return "";

            DateTime? dt = val.ToSDate();
            return dt == null ? "" : ((DateTime)dt).ToString("M/d/yyyy h:mm tt");
        }

        /// <summary>
        /// Return the javascript ticks for the given nullable datetime
        /// </summary>
        /// <param name="val">Nullable datetime to convert</param>
        /// <returns>Equivalent long</returns>
        public static long ToJavascriptTicks(this DateTime? val)
        {
            if (val == null)
                return default(long);

            return ((DateTime)val).ToJavascriptTicks();
        }

        private const long TicksToMillisOffset = 621355968000000000;

        /// <summary>
        /// Return the javascript ticks for the given datetime
        /// </summary>
        /// <param name="val">Datetime to convert</param>
        /// <returns>Equivalent long</returns>
        public static long ToJavascriptTicks(this DateTime val)
        {
            return (val.Ticks - TicksToMillisOffset) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Converts the given object to the given type
        /// </summary>
        /// <typeparam name="T">Desired type</typeparam>
        /// <param name="val">Object to convert</param>
        /// <returns>Equivalent type</returns>
        public static T ToSType<T>(this object val)
        {
            if (val == null || Convert.IsDBNull(val) || (val as string) == "")
                return default(T);

            try
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Trims the given string, then calls IsNullOrEmpty
        /// </summary>
        /// <param name="val">String to check</param>
        /// <returns>True if it is, false if not</returns>
        public static bool IsNullOrEmptyTrimmed(this string val)
        {
            if (val == null)
                return true;

            return string.IsNullOrEmpty(val.Trim()) || val.Equals("null", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Converts the given string into a format suitable for use on a query line
        /// </summary>
        /// <param name="val">String to convert</param>
        /// <returns>Converted string</returns>
        public static string ToQueryString(this string val)
        {
            if (val == null || Convert.IsDBNull(val))
                return "";

            return val.ToSStr().Replace(' ', '-').Replace('/', '-');
        }

        public static string ToQueryString2(this string val)
        {
            if (val == null || Convert.IsDBNull(val))
                return "";

            return val.ToSStr().Replace(' ', '-').Replace('/', '_');
        }

        private static readonly Regex nameRegex = new Regex(@"^[a-zA-Z\s-'\.]+$", RegexOptions.Compiled);

        /// <summary>
        /// Checks to see if the given string is a valid name
        /// </summary>
        /// <param name="val">Name to check</param>
        /// <returns>True if it is, false if not</returns>
        public static bool IsValidName(this string val)
        {
            if (val.IsNullOrEmptyTrimmed())
                return true;

            return nameRegex.IsMatch(val);
        }

        private static readonly Regex zipRegex = new Regex(@"^\d{5}([\-]\d{4})?$", RegexOptions.Compiled);

        /// <summary>
        /// Checks to see if the given string is a valid zip code
        /// </summary>
        /// <param name="val">Zip code to check</param>
        /// <returns>True if it is, false if not</returns>
        public static bool IsValidZip(this string val)
        {
            if (val.IsNullOrEmptyTrimmed())
                return true;

            return zipRegex.IsMatch(val);
        }

        /// <summary>
        /// Checks to see if the given string is a valid url (starts with http://, is longer than 11 characters [http://a.a.a])
        /// </summary>
        /// <param name="val">String to check</param>
        /// <returns>True if it is, false if not</returns>
        public static bool IsValidUrl(this string val)
        {
            return IsValidUrl(val, false);
        }

        /// <summary>
        /// Checks to see if the given string is a valid url (starts with http://, is longer than 11 characters [http://a.a.a])
        /// </summary>
        /// <param name="val">String to check</param>
        /// <param name="allowHTTPS">Whether to allow https</param>
        /// <returns>True if it is, false if not</returns>
        public static bool IsValidUrl(this string val, bool allowHTTPS)
        {
            if (allowHTTPS)
            {
                if (val.IsNullOrEmptyTrimmed() || val.Length < 12)
                {
                    return false;
                }

                if (val.ToLower().IndexOf("http://", StringComparison.Ordinal) == -1)
                {
                    if (val.ToLower().IndexOf("https://", StringComparison.Ordinal) == -1)
                        return false;
                    return true;
                }

                return true;
            }

            if (val.IsNullOrEmptyTrimmed() || val.ToLower().IndexOf("http://", StringComparison.Ordinal) == -1 || val.Length < 12)
                return false;

            return true;
        }

        /// <summary>
        /// Converts the given string to Title Case
        /// </summary>
        /// <param name="s">String to convert</param>
        /// <returns>Converted string</returns>
        public static string ToTitleCase(this string s)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s);
        }

        /// <summary>
        /// Converts the given string to Sentence case
        /// </summary>
        /// <param name="s">String to convert</param>
        /// <returns>Converted string</returns>
        public static string ToSentenceCase(this string s)
        {
            if (s.IsNullOrEmptyTrimmed())
                return string.Empty;

            return s.Length > 1 ? char.ToUpper(s[0]) + s.Substring(1).ToLower() : s.ToUpper();
        }

        /// <summary>
        /// Converts the given string to Sentence case - considers accronyms
        /// </summary>
        /// <param name="s"></param>
        /// <param name="hasAccronymes"></param>
        /// <returns></returns>
        public static string ToSentenceCase(this string s, bool hasAccronymes)
        {
            string converted = s;

            if (s.IsNullOrEmptyTrimmed())
            {
                return string.Empty;
            }

            if (s.Contains("(") && s.Contains(")") && hasAccronymes)
            {
                converted = s.Substring(0, s.IndexOf('('));
                int pos = s.IndexOf('(');
                string accr = s.Substring(pos);

                return converted = converted[0].ToString().ToUpper() + converted.Substring(1).ToLower() + accr;
            }
            else
            {
                converted = s.ToLower();

                return converted = converted[0].ToString().ToUpper() + converted.Substring(1);
            }
        }

        /// <summary>
        /// Converts the given string from a useragent string to a browser string
        /// </summary>
        /// <param name="s">Useragent string</param>
        /// <returns>Equivalent Browser</returns>
        public static string ToBrowserString(this string s)
        {
            if (s.IsNullOrEmptyTrimmed())
                return "Unsupported Browser";

            try
            {
                if (s.IndexOf("MSIE", StringComparison.Ordinal) != -1) //IE
                {
                    //find MSIE, and version is right after a space
                    return "Internet Explorer " + s.Substring(s.IndexOf("MSIE", StringComparison.Ordinal) + 5, 3);
                }

                if (s.IndexOf("Firefox", StringComparison.Ordinal) != -1) //Firefox
                {
                    //find Firefox, and version is right after a slash
                    return "Firefox " + s.Substring(s.IndexOf("Firefox", StringComparison.Ordinal) + 8);
                }

                if (s.IndexOf("Chrome", StringComparison.Ordinal) != -1) //Chrome
                {
                    //find Chrome, and version is right after a slash
                    int idx = s.IndexOf("Chrome", StringComparison.Ordinal) + 7,
                        eidx = s.IndexOf(" ", idx, StringComparison.Ordinal);
                    return "Chrome " + (eidx > 0 ? s.Substring(idx, eidx - idx) : s.Substring(idx));
                }

                if (s.IndexOf("Version", StringComparison.Ordinal) != -1) //Safari
                {
                    //find Version, and version is right after a slash
                    int idx = s.IndexOf("Version", StringComparison.Ordinal) + 8,
                        eidx = s.IndexOf(" ", idx, StringComparison.Ordinal);
                    return "Safari " + (eidx > 0 ? s.Substring(idx, eidx - idx) : s.Substring(idx));
                }

                return "Unsupported Browser";
            }
            catch
            {
                return "Unsupported Browser";
            }
        }

        /// <summary>
        /// Takes in a collection of form data and sends it back as a delimited string
        /// </summary>
        /// <param name="formData">Form data</param>
        /// <returns>Equivalent String</returns>
        public static string SerializeData(this FormCollection formData)
        {
            if (formData == null || formData.Keys.Count == 0)
                return "";

            List<string> l = new List<string>(formData.Keys.Count);
            foreach (string s in formData.Keys)
            {
                if (s != null && s.IndexOf("viewstate", StringComparison.CurrentCultureIgnoreCase) != -1)
                    continue;

                l.Add(string.Format("{0}={1}", s.IsNullOrEmptyTrimmed() ? string.Empty : s, formData[s].IsNullOrEmptyTrimmed() ? string.Empty : formData[s]));
            }

            return l.ListToString('|');
        }

        /// <summary>
        /// Limits the given string to the given length
        /// </summary>
        /// <param name="val">String to limit</param>
        /// <param name="len">Length limit</param>
        /// <returns>Limited string</returns>
        public static string LimitLength(this string val, int len)
        {
            if (val.IsNullOrEmptyTrimmed() || val.Length <= len)
                return val;

            return val.Substring(0, len - 3) + "...";
        }

        /// <summary>
        /// Takes in a collection of form data and sends it back as a series of inputs
        /// </summary>
        /// <param name="formData">Form data</param>
        /// <returns>Equivalent inputs</returns>
        public static string SerializeToInputs(this FormCollection formData)
        {
            if (formData == null || formData.Keys.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (string s in formData.Keys)
            {
                sb.AppendFormat("<input type='hidden' name='{0}' value='{1}'/>", s, formData[s]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Takes in a string representing form data and returns it as a collection of form data
        /// </summary>
        /// <param name="formData">Form data string</param>
        /// <returns>Equivalent form collection</returns>
        public static FormCollection DeserializeData(this string formData)
        {
            if (formData.IsNullOrEmptyTrimmed())
                return new FormCollection();

            FormCollection coll = new FormCollection();
            foreach (string pair in formData.Split('|'))
            {
                string[] tmp = pair.Split('=');
                if (tmp.Length == 2)
                    coll.Add(tmp[0], tmp[1]);
                else
                    coll.Add(tmp[0], "");
            }

            return coll;
        }

        /// <summary>
        /// Checks to see if the given email address is an internal one
        /// </summary>
        /// <param name="val">Email address to check</param>
        /// <returns>True if it is, false if not</returns>
        public static bool IsInternalEmail(this string val)
        {
            if (val.IsNullOrEmptyTrimmed())
                return false;

            return (val.IndexOf("@wellclicks.com", StringComparison.Ordinal) != -1 || val.IndexOf("@createhealthinc.com", StringComparison.Ordinal) != -1 || val.ToLower().IndexOf("bclarke@impactqualit.com", StringComparison.Ordinal) != -1);
        }

        /// <summary>
        /// Computes a hash for the given bytes
        /// </summary>
        /// <param name="val">Bytes to compute on</param>
        /// <returns>Hash</returns>
        public static string ComputeHash(this byte[] val)
        {
            if (val == null)
                return "";

            byte[] hash = MD5.Create().ComputeHash(val);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }

        /// <summary>
        /// Computes a hash for the given string
        /// </summary>
        /// <param name="val">String to compute</param>
        /// <returns>Hash</returns>
        public static string ComputeHash(this string val)
        {
            if (val.IsNullOrEmptyTrimmed())
                return "";

            return ASCIIEncoding.ASCII.GetBytes(val).ComputeHash();
        }

        private static readonly Regex htmlRegex = new Regex("<[^>]+>|&nbsp;", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Based on code from http://stackoverflow.com/questions/731649/how-can-i-convert-html-to-text-in-c
        /// </summary>
        /// <param name="text">Text to strip</param>
        /// <returns>Stripped text</returns>
        public static string StripHTML(this string text)
        {
            return htmlRegex.Replace(HttpUtility.HtmlDecode(text ?? ""), "").Split('<')[0];
        }

        private static readonly Regex breakRegex = new Regex(@"\<[Bb][Rr]\s*\>", RegexOptions.Compiled);

        /// <summary>
        /// Fixes the break tags so they're XML compliant
        /// </summary>
        /// <param name="text">Text to fix</param>
        /// <returns>Fixed text</returns>
        public static string FixBreaks(this string text)
        {
            return breakRegex.Replace(text ?? "", "<br/>");
        }

        private static readonly Regex attrRegex = new Regex("\\s+(\\w*)=([^\" >]+)", RegexOptions.Compiled);

        /// <summary>
        /// Fixes the attributes so they're XML compliant
        /// </summary>
        /// <param name="text">Text to fix</param>
        /// <returns>Fixed text</returns>
        public static string FixAttrs(this string text)
        {
            return attrRegex.Replace(text ?? "", " $1=\"$2\"");
        }

        private static readonly Regex extendedRegex = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);

        /// <summary>
        /// Strips all extended characters from the given text
        /// </summary>
        /// <param name="text">Text to strip</param>
        /// <returns>Stripped text</returns>
        public static string StripExtendedChars(this string text)
        {
            return extendedRegex.Replace(text ?? "", "");
        }

        private static readonly Regex specialRegex = new Regex(@"[^\w. -/(),–|]", RegexOptions.Compiled);

        /// <summary>
        /// Strips all characters that aren't standard alphanumeric
        /// </summary>
        /// <param name="text">Text to strip</param>
        /// <returns>Stripped text</returns>
        public static string StripSpecialChars(this string text)
        {
            return specialRegex.Replace(text ?? "", "");
        }

        /// <summary>
        /// Sanitizes input coming in to make sure it won't cause any issues
        /// </summary>
        /// <param name="text">Text to clean</param>
        /// <returns>Sanitized text</returns>
        public static string sanitizeField(this string text)
        {
            if (text.IsNullOrEmptyTrimmed())
                return "";

            return text.StripSpecialChars().Trim();
        }

        private static readonly Regex base64Regex = new Regex(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{4})$", RegexOptions.Compiled);

        /// <summary>
        /// Checks to see if the given string is valid base64 data
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool isBase64(this string text)
        {
            return (text.Length % 4) == 0 && base64Regex.IsMatch(text);
        }

        /// <summary>
        /// Adds a double quote to the end if there are unbalance double quotes
        /// </summary>
        /// <param name="text">Text to balance</param>
        /// <returns>Balanced text</returns>
        public static string BalanceQuotes(this string text)
        {
            if (text.IndexOf("\"", StringComparison.Ordinal) == -1)
                return text;

            int cnt = 0;
            foreach (char c in text)
                if (c == '"')
                    cnt++;

            if ((cnt % 2) != 0)
                return text + "\"";

            return text;
        }

        /// <summary>
        /// Converts a boolean to yes/no
        /// </summary>
        /// <param name="val">Bool to convert</param>
        /// <returns>Yes or no</returns>
        public static string ToYesNo(this bool val)
        {
            return val ? "Yes" : "No";
        }

        /// <summary>
        /// Converts a nullable boolean to yes/no
        /// </summary>
        /// <param name="val">Nullable bool to convert</param>
        /// <returns>Yes, no, or blank</returns>
        public static string ToYesNo(this bool? val)
        {
            return val == null ? "" : val == true ? "Yes" : "No";
        }

        /// <summary>
        /// Trims off whatever is in parentheses
        /// </summary>
        /// <param name="val">String to trim</param>
        /// <returns>Trimmed string</returns>
        public static string TrimParenthetical(this string val)
        {
            if (val.IsNullOrEmptyTrimmed())
                return "";

            int idx;
            if ((idx = val.IndexOf("(", StringComparison.Ordinal)) != -1)
                return val.Substring(0, idx).Trim();

            return val.Trim();
        }

        /// <summary>
        /// Calculates the next date based on the given date and frequency
        /// </summary>
        /// <param name="curDate">Date</param>
        /// <param name="frequency">Frequency</param>
        /// <returns>Next date</returns>
        public static DateTime CalculateNextDate(this DateTime? curDate, string frequency)
        {
            return CalculateNextDateInternal(curDate, frequency);
        }

        /// <summary>
        /// Calculates the next date based on the given date and frequency
        /// </summary>
        /// <param name="curDate">Date</param>
        /// <param name="frequency">Frequency</param>
        /// <returns>Next date</returns>
        public static DateTime CalculateNextDate(this DateTime curDate, string frequency)
        {
            return CalculateNextDateInternal(curDate, frequency);
        }

        /// <summary>
        /// Calculates the next date based on the given date and frequency
        /// </summary>
        /// <param name="curDate">Date</param>
        /// <param name="frequency">Frequency</param>
        /// <returns>Next date</returns>
        private static DateTime CalculateNextDateInternal(this DateTime? curDate, string frequency)
        {
            if (curDate == null || curDate.Value.Date == DateTime.MaxValue.Date)
                return DateTime.Today;

            switch (frequency.ToLower())
            {
                case "daily":
                    return ((DateTime)curDate).AddDays(1);
                case "week":
                case "weekly":
                    return ((DateTime)curDate).AddDays(7);
                case "month":
                case "monthly":
                    return ((DateTime)curDate).AddMonths(1);
                case "quarter":
                case "quarterly":
                case "threemonth":
                    return ((DateTime)curDate).AddMonths(3);
                case "sixmonth":
                case "semi-annually":
                case "semianually":
                    return ((DateTime)curDate).AddMonths(6);
                case "year":
                case "yearly":
                    return ((DateTime)curDate).AddYears(1);
                case "never":
                    return DateTime.MaxValue;
                case "once":
                    return DateTime.Today.AddDays(-1);
                default:
                    return DateTime.Today;
            }
        }

        /// <summary>
        /// Gets the first week day following the current date.
        /// </summary>
        /// <param name="date">the date</param>
        /// <param name="dayOfWeek">the day of the week to return</param>
        /// <returns>the first dayOfWeek day</returns>
        public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }


        public static DateTime TimeRoundUp(DateTime input, int minutes)
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0).AddMinutes(input.Minute % minutes == 0 ? 0 : minutes - input.Minute % minutes);
        }

        public static DateTime TimeRoundDown(DateTime input, int minutes)
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0).AddMinutes(-input.Minute % minutes);
        }

        /// <summary>
        /// Wraps the given text with percentage signs for like queries
        /// </summary>
        /// <param name="text">Text to wrap</param>
        /// <returns>Wrapped text</returns>
        public static string WrapForLike(this string text)
        {
            return WrapForLike(text, true);
        }

        /// <summary>
        /// Wraps the given text with percentage signs for like queries
        /// </summary>
        /// <param name="text">Text to wrap</param>
        /// <param name="doAll">Do both sides</param>
        /// <returns>Wrapped text</returns>
        public static string WrapForLike(this string text, bool doAll)
        {
            return WrapForLike(text, doAll, true);
        }

        /// <summary>
        /// Wraps the given text with percentage signs for like queries
        /// </summary>
        /// <param name="text">Text to wrap</param>
        /// <param name="doAll">Do both sides</param>
        /// <param name="doReplace">Replace single quotes (use false if wrapping for use as a parameter)</param>
        /// <returns>Wrapped text</returns>
        public static string WrapForLike(this string text, bool doAll, bool doReplace)
        {
            return (doAll ? "%" : "") + (doReplace ? text.ToSStr().Replace("''", "'").Replace("'", "''") : text.ToSStr()) + "%";
        }

        /// <summary>
        /// Gets the requested value from the given node
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="xpath">Value to get</param>
        /// <returns>Value</returns>
        public static string getValueFromXml(this XmlNode node, string xpath)
        {
            XmlNode found = node.SelectSingleNode(xpath);
            return found == null ? "" : found.InnerText;
        }

        /// <summary>
        /// Gets the requested attribute from the given node
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="attribute">Attribute to get</param>
        /// <returns>Attribute</returns>
        public static string getAttributeFromXml(this XmlNode node, string attribute)
        {
            if (node == null)
                return "";

            if (node.Attributes != null)
            {
                XmlAttribute attr = node.Attributes[attribute];
                return attr == null ? "" : attr.Value;
            }

            return null;
        }

        /// <summary>
        /// Splits the given camel case string into separate words
        /// </summary>
        /// <param name="val">Word to split</param>
        /// <returns>Split word</returns>
        public static string splitCamelCase(this string val)
        {
            char[] origChars = val.ToCharArray();
            int len = origChars.Length;
            List<char> finalChars = new List<char>(len);

            for (int i = 0; i < len; i++)
            {
                char c = origChars[i];
                if ((int)c > 64 && (int)c < 91) //it's an upper-case word
                    finalChars.Add(' ');
                finalChars.Add(c);
            }

            return new string(finalChars.ToArray());
        }

        /// <summary>
        /// Checks to see if the date is less than or equal to the minimum value
        /// </summary>
        /// <param name="val">Date to check</param>
        /// <returns>True if it is, false if not</returns>
        public static bool isMinValue(this DateTime val)
        {
            return (val.CompareTo((DateTime)SqlDateTime.MinValue) < 1);
        }

        /// <summary>
        /// Checks to see if the date is less than or equal to the minimum value
        /// </summary>
        /// <param name="val">Date to check</param>
        /// <returns>True if it is, false if not</returns>
        public static bool isNullOrMinValue(this DateTime? val)
        {
            if (val == null)
                return true;

            return (((DateTime)val).CompareTo((DateTime)SqlDateTime.MinValue) < 1);
        }

        /// <summary>
        /// Clientize's a brand's type
        /// </summary>
        /// <param name="brandType">Brand type</param>
        /// <param name="clientName">Client name</param>
        /// <returns>Clientized brand type</returns>
        public static string clientizeBrandType(this string brandType, string clientName)
        {
            switch (brandType.ToLower())
            {
                case "non-owned hospital":
                    return "Other Hospital";
                case "hospital":
                    return clientName + " Hospital";
                default:
                    return brandType;
            }
        }

        /// <summary>
        /// Converts any object to JSON
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>JSON representation</returns>
        public static string convertToJSON(this object o)
        {
            if (o == null)
                return "";

            return JsonConvert.SerializeObject(o);
        }

        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }

        public static string ShrinkWrapText(this string sInput, int breakPointWidth, bool isHtml)
        {
            char[] splitChars = { ' ', '-', '\t' };
            try
            {
                string[] words = ExplodeString(sInput, splitChars);
                int nCurrentLineLength = 0;
                StringBuilder sbText = new StringBuilder();
                for (int i = 0; i < words.Length; i += 1)
                {
                    string word = words[i];
                    if (nCurrentLineLength + word.Length > breakPointWidth)
                    {
                        if (nCurrentLineLength > 0)
                        {
                            if (isHtml)
                            {
                                sbText.Append("<br />");
                            }
                            else
                            {
                                sbText.Append(Environment.NewLine);
                            }

                            nCurrentLineLength = 0;
                        }

                        while (word.Length > breakPointWidth)
                        {
                            sbText.Append(word.Substring(0, breakPointWidth - 1) + "-");
                            word = word.Substring(breakPointWidth - 1);
                            if (isHtml)
                            {
                                sbText.Append("<br />");
                            }
                            else
                            {
                                sbText.Append(Environment.NewLine);
                            }
                        }

                        word = word.TrimStart();
                    }

                    sbText.Append(word);
                    nCurrentLineLength += word.Length;
                }

                return sbText.ToString();
            }
            catch (Exception)
            {
                return sInput;
            }
        }

        public static string[] ExplodeString(string sInput, char[] splitChars)
        {
            List<string> parts = new List<string>();
            int startIndex = 0;
            while (true)
            {
                int index = sInput.IndexOfAny(splitChars, startIndex);

                if (index == -1)
                {
                    parts.Add(sInput.Substring(startIndex));
                    return parts.ToArray();
                }

                string word = sInput.Substring(startIndex, index - startIndex);
                char nextChar = sInput.Substring(index, 1)[0];
                if (char.IsWhiteSpace(nextChar))
                {
                    parts.Add(word);
                    parts.Add(nextChar.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    parts.Add(word + nextChar);
                }

                startIndex = index + 1;
            }
        }
    }
}