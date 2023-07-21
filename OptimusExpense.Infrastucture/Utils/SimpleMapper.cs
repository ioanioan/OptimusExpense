using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace OptimusExpense.Infrastucture.Utils
{

    public static class SimpleMapper
    {

        public static T ConvertDynamic<T>(dynamic data)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));
        }
            
    public static void PropertyMap<T, U>(T source, U destination)
            where T : class, new()
            where U : class, new()
        {
            List<PropertyInfo> sourceProperties = source.GetType().GetProperties().ToList<PropertyInfo>();
            List<PropertyInfo> destinationProperties = destination.GetType().GetProperties().ToList<PropertyInfo>();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                PropertyInfo destinationProperty = destinationProperties.Find(item => item.Name == sourceProperty.Name);

                if (destinationProperty != null && destinationProperty.CanWrite)
                {
                    try
                    {
                        destinationProperty.SetValue(destination, sourceProperty.GetValue(source, null), null);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
        }

        public static DateTime GetDateFirstWeekMonth(this DateTime dt)
        {
            DateTime dtFirst = new DateTime(dt.Year, dt.Month, 1);
            var nr = dtFirst.FirstDayWeek() - 1;

            return dtFirst.AddDays(-nr);
        }

        public static DateTime GetDateLastWeekMonth(this DateTime dt)
        {
            DateTime dtLast = new DateTime(dt.Year, dt.Month, 1).AddMonths(1);
            var nr = 7 - dtLast.FirstDayWeek();

            return dtLast.AddDays(nr);
        }

        private static int FirstDayWeek(this DateTime dt)
        {
            return (dt.DayOfWeek == DayOfWeek.Sunday ? 7 : dt.DayOfWeek.GetHashCode());
        }

        public static String ZiRomana(this DateTime dt)
        {
            var nr = dt.DayOfWeek.GetHashCode();
            var zile = new String[] { "Duminica", "Luni", "Marti", "Miercuri", "Joi", "Vineri", "Sambata" };
            return zile[nr];
        }

        public static String LunaRomana(this DateTime dt)
        {
            var nr = dt.Month - 1;
            var luni = new String[] { "Ian", "Feb", "Mar", "Apr", "Mai", "Iun", "Iul", "Aug", "Sep", "Oct", "Noi", "Dec" };
            return luni[nr];
        }

        public static String ReplaceAfter(this string valoare, String chars)
        {
            if (valoare.Contains(chars))
            {
                return valoare.Substring(0, valoare.IndexOf(chars));
            }
            return valoare;
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static String FormatSeparatorNumber(this int myInt)
        {
            NumberFormatInfo nfi = Thread.CurrentThread.CurrentUICulture.NumberFormat;

            return myInt.ToString("N0", nfi);
        }

        public static String FormatSeparatorNumber(this long myInt)
        {
            NumberFormatInfo nfi = Thread.CurrentThread.CurrentUICulture.NumberFormat;

            return myInt.ToString("N0", nfi);
        }

        public static String FormatSeparatorNumber(this decimal myInt)
        {
            NumberFormatInfo nfi = Thread.CurrentThread.CurrentUICulture.NumberFormat;

            return myInt.ToString("N2", nfi);
        }

        public static Type GetType<T>(String extensions)
        {
            Type type = typeof(T);

            Type newtype = null;
            try
            {
                newtype = type.Assembly.DefinedTypes.Where(p => p.Name.Contains(type.Name.Replace("`1", "")) && p.Name.Contains("_" + extensions)).FirstOrDefault();
            }
            catch
            {

            }

            return newtype != null ? newtype : type;

        }

        public static String C2(this decimal valoare)
        {
            return valoare.ToString("c2", new CultureInfo("ro-RO")).Replace("lei", "RON");
        }

        public static string HexToStr(this string hexString)
        {

            string s = hexString;

            string result = string.Empty;



            int i = 0;

            int j = 1;



            while (i < s.Length)
            {

                result = result + Convert.ToChar(Convert.ToInt32("0x" + s.Substring(i, 2), 16) ^ 7);

                i = i + 2;

                j = j + 1;

            }



            return result;

        }

        public static String ExtractDigits(this string value)
        {
            return Regex.Match(value, @"\d+").Value;
        }

        public static long ExtractDigitsLong(this string value)
        {
            return ExtractDigitsLong(value);
        }


        public static Guid ToGuid(this string src)
        {
            if (src == null)
            {
                return new Guid();
            }
            byte[] stringbytes = Encoding.UTF8.GetBytes(src);
            byte[] hashedBytes = new System.Security.Cryptography
                .SHA1CryptoServiceProvider()
                .ComputeHash(stringbytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }


        public static String ToGuidString(this string src)
        {
            return ToGuid(src).ToString();
        }

        public static void SetColumnsOrder(this DataTable dtbl, params String[] columnNames)
        {
            List<string> listColNames = columnNames.ToList();

            //Remove invalid column names.
            foreach (string colName in columnNames)
            {
                if (!dtbl.Columns.Contains(colName))
                {
                    listColNames.Remove(colName);
                }
            }

            foreach (string colName in listColNames)
            {
                dtbl.Columns[colName].SetOrdinal(listColNames.IndexOf(colName));
            }
        }
    }
}