using LeapSpring.MJC.Core.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LeapSpring.MJC.Core
{
    /// <summary>
    /// Represents the extension
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Returns Enum(ed) entries as Dictionary
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static Dictionary<int, string> EnumToDictionary(this Enum @enum)
        {
            var type = @enum.GetType();
            return Enum.GetValues(type).Cast<int>().ToDictionary(e => e, e => Enum.GetName(type, e));
        }

        /// <summary>
        /// Returns Enum(ed) entries as Dictionary
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static IList<SelectItem> EnumToSelectItem(this Enum @enum)
        {
            var type = @enum.GetType();
            return Enum.GetValues(type).Cast<int>().ToDictionary(e => e, e => Enum.GetName(type, e)).Select(p =>
            {
                return new SelectItem
                {
                    Id = p.Key,
                    Name = p.Value
                };
            }).ToList();
        }

        /// <summary>
        /// Gets enum description
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>string</returns>
        public static string GetEnumDescriptionValue(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes != null && attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Gets enum value from description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description">description</param>
        /// <returns>string</returns>
        public static T GetEnumFromDescription<T>(this string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default(T);
        }

        /// <summary>
        /// String[Description] to enum
        /// </summary>
        /// <typeparam name="T">Defalt type</typeparam>
        /// <param name="stringValue">Description</param>
        /// <param name="defaultValue">Default type</param>
        /// <returns>Result</returns>
        public static T ToEnum<T>(this string stringValue, T defaultValue)
        {
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                DescriptionAttribute[] da = (DescriptionAttribute[])(typeof(T).GetField(enumValue.ToString()))
                    .GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (da.Length > 0 && da[0].Description == stringValue)
                    return enumValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// First character to upper
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Result</returns>
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }

        /// <summary>
        /// Random element
        /// </summary>
        /// <typeparam name="T">Queryable</typeparam>
        /// <param name="q"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T RandomElement<T>(this IQueryable<T> q, Expression<Func<T, bool>> e = null)
        {
            var r = new Random();
            if (e != null)
                q = q.Where(e);

            return q.OrderBy(m => true).Skip(r.Next(q.Count())).FirstOrDefault();
        }

        /// <summary>
        /// Convert short day name to full name
        /// </summary>
        /// <param name="shortForm">Short form</param>
        /// <returns>Full name</returns>
        public static string ConvertToDayFullName(this string shortForm)
        {
            var dayShortNames = shortForm.Replace(" ", "").Split(',');
            var fullName = string.Empty;

            foreach (var dayShortName in dayShortNames)
            {
                switch (dayShortName)
                {
                    case "m":
                    case "mon":
                    case "monday":
                        fullName += "Monday,";
                        break;
                    case "t":
                    case "tue":
                    case "tuesday":
                        fullName += "Tuesday,";
                        break;
                    case "w":
                    case "wed":
                    case "wednesday":
                        fullName += "Wednesday,";
                        break;
                    case "th":
                    case "thu":
                    case "thursday":
                        fullName += "Thursday,";
                        break;
                    case "f":
                    case "fri":
                    case "friday":
                        fullName += "Friday,";
                        break;
                    case "s":
                    case "sa":
                    case "sat":
                    case "saturday":
                        fullName += "Saturday,";
                        break;
                    case "su":
                    case "sun":
                    case "sunday":
                        fullName += "Sunday,";
                        break;
                    case "today":
                        fullName += DateTime.UtcNow.ToString("dddd") + ",";
                        break;
                    case "all":
                        return "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday";
                }
            }

            return fullName.TrimEnd(',');
        }

        /// <summary>
        /// Removes the country code from phone number
        /// </summary>
        /// <param name="phoneNumber">The phone numeber.</param>
        /// <returns>The formatted phone number</returns>
        public static string RemoveCountyCode(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;

            phoneNumber = phoneNumber.Replace("+", "");

            // Indian phone number
            if (phoneNumber.Length == 12)
                return phoneNumber;

            if ((phoneNumber.Length - 10 == -1))
                return phoneNumber;

            // Other phone numbers
            return (phoneNumber.Length == 10) ? phoneNumber : phoneNumber.Substring(phoneNumber.Length - 10);
        }

        /// <summary>
        /// Appends the country code from phone number
        /// </summary>
        /// <param name="phoneNumber">The phone numeber.</param>
        /// <returns>The formatted phone number</returns>
        public static string AppendCountyCode(this string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return phoneNumber;

            if (phoneNumber.StartsWith("+1"))
                return phoneNumber;

            var countryCode = (phoneNumber.Length == 10) ? "+1" : "+";
            phoneNumber = phoneNumber.Replace("+", "");

            return string.Format("{0}{1}", countryCode, phoneNumber.RemoveCountyCode());
        }

        /// <summary>
        /// Converts the string of date format to UTC date time.
        /// </summary>
        /// <param name="date">The date string</param>
        /// <returns>The UTC date time.</returns>
        public static DateTime ToUTCTime(this string date)
        {
            var localDate = Convert.ToDateTime(date);
            var currentTime = TimeZoneInfo.ConvertTime(localDate, TimeZoneInfo.Local, TimeZoneInfo.Utc);
            return TimeZoneInfo.ConvertTimeToUtc(currentTime, TimeZoneInfo.Utc);
        }

        /// <summary>
        /// Gets the next payday
        /// </summary>
        /// <param name="todayDate">the today's date</param>
        /// <param name="payday">The pay day to set</param>
        /// <returns>The next payday</returns>
        public static DateTime GetNextPayDay(this DateTime todayDate, DayOfWeek? payday = null, int addedDay = 0)
        {
            payday = payday.HasValue ? payday : DayOfWeek.Thursday;

            int daysToAdd = ((int)payday - (int)todayDate.DayOfWeek + 7) % 7;
            if (addedDay != 0)
                daysToAdd = daysToAdd + 7;
            return todayDate.AddDays(daysToAdd).Date.AddHours(15);
        }

        /// <summary>
        /// Gets the last payday
        /// </summary>
        /// <param name="todayDate">the today's date</param>
        /// <returns>The next payday</returns>
        public static DateTime GetLastPayDay(this DateTime todayDate)
        {
            int daysToAdd = (((int)DayOfWeek.Thursday - (int)todayDate.DayOfWeek + 7) % 7) - 7;
            return todayDate.AddDays(daysToAdd);
        }

        public static DateTime getNextPendingStatusday(this DateTime todayDate)
        {
            int daysToAdd = (((int)DayOfWeek.Thursday - (int)todayDate.DayOfWeek) / 7) * 7 + 10;
            return todayDate.Date.AddDays(daysToAdd);
        }

        public static DateTime getLastPendingStatusday(this DateTime todayDate)
        {
            int daysToAdd = (((int)DayOfWeek.Thursday - (int)todayDate.DayOfWeek) / 7) * 7 + 3;
            return todayDate.Date.AddDays(daysToAdd);
        }

        public static DateTime getEndDate(this DateTime todayDate)
        {
            if (todayDate.DayOfWeek == DayOfWeek.Thursday && todayDate.Hour >= 10)
                todayDate = todayDate.AddDays(+1);

            while (todayDate.DayOfWeek != DayOfWeek.Thursday)
                todayDate = todayDate.AddDays(+1);

            DateTime dt = todayDate.Date;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            bool isDST = tzi.IsDaylightSavingTime(dt);

            if (isDST)
                return todayDate.Date.AddHours(15);
            else
                return todayDate.Date.AddHours(17);
        }

        public static DateTime getStartDate(this DateTime todayDate)
        {
            //int daysToAdd = ((int)(int)DayOfWeek.Thursday - (int)todayDate.DayOfWeek + 7) % 7;

            // todayDate = DateTime.UtcNow;

            DateTime lastThursday = todayDate.Date;
            if (todayDate.DayOfWeek == DayOfWeek.Thursday && todayDate.Hour >= 10)
            {
                lastThursday = lastThursday.Date;
            }
            else
            {
                if (todayDate.DayOfWeek == DayOfWeek.Thursday && todayDate.Hour <= 10)
                    lastThursday = lastThursday.AddDays(-1);

                while (lastThursday.DayOfWeek != DayOfWeek.Thursday)
                    lastThursday = lastThursday.AddDays(-1);
            }
            DateTime dt = lastThursday.Date;
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            bool isDST = tzi.IsDaylightSavingTime(dt);

            if (isDST)
                return lastThursday.Date.AddHours(15);
            else
                return lastThursday.Date.AddHours(17);
        }

        /// <summary>
        /// Gets the payday time notification time.
        /// </summary>
        /// <returns>The payday time notification time</returns>
        public static TimeSpan GetPaydayTime()
        {
            return TimeSpan.Parse("13:00");
        }

        /// <summary>
        /// Converts to payday time
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>The date time</returns>
        public static DateTime ToPayDayTime(this DateTime dateTime)
        {
            var paydayTime = GetPaydayTime();
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                paydayTime.Hours,
                paydayTime.Minutes,
                paydayTime.Seconds,
                paydayTime.Milliseconds,
                dateTime.Kind);
        }
    }
}
