using System;
using System.Globalization;

namespace phanmemnguyenhue.helper
{
    public static class DateTimeHelper
    {
        public static DateTime ConvertStringToDateTime(this string dateTimeString, string stringFormat = "dd/MM/yyyy")
        {
            DateTime result = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTimeString) && !string.IsNullOrEmpty(stringFormat))
            {
                DateTime.TryParseExact(dateTimeString, stringFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out result);
            }
            return result;
        }

        public static string DateTimeToString(this DateTime dt, string textEmpty = "", string format = "dd/MM/yyyy")
        {
            return dt == DateTime.MinValue ? textEmpty : dt.ToString(format);
        }

        public static string TimeAgo(this DateTime dt, string format = "dd/MM/yyy HH:mm:ss")
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days <= 0)
            {
                if (span.Hours > 0)
                    return string.Format(" {0} {1} trước",
                        span.Hours, "giờ");
                if (span.Minutes > 0)
                    return string.Format(" {0} {1} trước",
                        span.Minutes, "phút");
                if (span.Seconds > 5)
                    return string.Format(" {0} giây trước", span.Seconds);
                if (span.Seconds <= 5)
                    return "vừa xong";
            }

            return DateTimeToString(dt, string.Empty, format);
        }

    }
}
