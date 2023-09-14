using System;

namespace www.worldisawesome.fun.Services
{
    public static class DateTimeHelpers
    {
        public static double? GetMillisecondsFromDateTime(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                return new DateTimeOffset((DateTime)dateTime).ToUnixTimeMilliseconds();
            } else
            {
                return null;
            }
        }
        public static DateTime? GetDateTimeFromMilliseconds(double? milliseconds)
        {
            if (milliseconds != null)
            {
                return DateTimeOffset.FromUnixTimeMilliseconds((long)milliseconds).DateTime;
            }
            else
            {
                return null;
            }
        }

        public static double? GetMillisecondsFromDateTimeOffset(DateTimeOffset? dateTimeOffset)
        {
            if (dateTimeOffset != null)
            {
                return (double)(dateTimeOffset?.ToUnixTimeMilliseconds());
            } else
            {
                return null;
            }
        }
        public static DateTimeOffset? GetDateTimeOffsetFromMilliseconds(double? milliseconds)
        {
            if (milliseconds != null)
            {
                return DateTimeOffset.FromUnixTimeMilliseconds((long)milliseconds);
            } else
            {
                return null;
            }
        }

        public static double? GetMillisecondsFromTimeSpan(TimeSpan? timeSpan)
        {
            return timeSpan?.TotalMilliseconds;
        }
        public static TimeSpan? GetTimeSpanFromMilliseconds(double? milliseconds)
        {
            if(milliseconds != null)
            {
                return TimeSpan.FromMilliseconds((double)milliseconds);
            } else
            {
                return null;
            }
        }

        public static string GetDateFormattedFromDateTime(DateTime? dateTime)
        {
            return dateTime?.ToString("yyyy-MM-dd");
        }
        public static DateTime? GetDateTimeFromDateFormatted(string dateTimeFormatted)
        {
            if (dateTimeFormatted != null)
            {
                return DateTime.Parse(dateTimeFormatted);
            } else
            {
                return null;
            }
        }

        public static string GetTimeFormattedFromTimeSpan(TimeSpan? timeSpan)
        {
            return timeSpan?.ToString(@"hh\:mm");
        }
        public static TimeSpan? GetTimeSpanFromTimeFormatted(string timeSpanFormatted)
        {
            if (timeSpanFormatted != null)
            {
                return TimeSpan.Parse(timeSpanFormatted);
            }
            else
            {
                return null;
            }
        }
    }
}
