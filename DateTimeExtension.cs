using System;

namespace SnctJikanwari
{
    public static class DateTimeExtension
    {
        public static uint ToTimeStamp(this DateTime dateTime)
        {
            var timeSpan = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (uint) timeSpan.TotalSeconds;
        }

        public static string ToTimeStampString(this DateTime dateTime)
        {
            var timeSpan = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ((uint) timeSpan.TotalSeconds).ToString();
        }

        public static DateTime ToLocalDateTime(this uint timeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(timeStamp)
                .ToLocalTime();
        }

        public static DateTime ToUtcDateTime(this uint timeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(timeStamp)
                .ToUniversalTime();
        }

        public static DateTime ToLocalDateTime(this string timeStampString)
        {
            var isSuccess = uint.TryParse(timeStampString, out var timeStamp);
            if (!isSuccess) throw new FormatException(timeStampString);

            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(timeStamp)
                .ToLocalTime();
        }

        public static DateTime ToUtcDateTime(this string timeStampString)
        {
            var isSuccess = uint.TryParse(timeStampString, out var timeStamp);
            if (!isSuccess) throw new FormatException(timeStampString);

            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(timeStamp)
                .ToUniversalTime();
        }
    }
}