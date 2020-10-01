using System;
using System.Collections.Generic;
using System.Linq;

namespace SnctJikanwari.JikanwariContents.Jugyo
{
    public class DefaultJugyo : IJugyo
    {
        public int Time { get; }
        public string TimeString => Time switch
        {
            1 => "8:45-10:15",
            2 => "10:20-11:50",
            3 => "13:10-14:40",
            4 => "14:50-16:20",
            5 => "16:30-18:00",
            _ => ""
        };
        public string Subject { get; }
        public string ClassName { get; }
        public string Teacher { get; }
        public string Other { get; }

        private DefaultJugyo(int time, string className, string timeRaw)
        {
            var details = timeRaw.Split(":");
            Time = time;
            Subject = details[0];
            ClassName = className;
            Teacher = details[1];
            Other = details[2];
        }

        private static List<DefaultJugyo> GetDailyJikanwari(string className, string dailyRaw)
        {
            var daily = new List<DefaultJugyo>();
            var timeDetails = dailyRaw.Split(',').Where(r => !string.IsNullOrEmpty(r));
            daily.AddRange(timeDetails.Select((tRaw, time) => new DefaultJugyo(time + 1, className, tRaw)));

            return daily;
        }

        private static List<List<DefaultJugyo>?> GerWeeklyJikanwari(string className, string rawData)
        {
            var weeklyJikanwari = new List<List<DefaultJugyo>?>();
            var dailyRaw = rawData.Split('\n');

            weeklyJikanwari.AddRange(dailyRaw.Select(dRaw =>
                string.IsNullOrEmpty(dRaw) ? null : GetDailyJikanwari(className, dRaw)));

            return weeklyJikanwari;
        }

        public static IEnumerable<DefaultJugyo>? GetDailyJikanwari(string className, DateTime date, string? rawData,
            out DayOfWeek advanceChanged)
        {
            if (rawData == null)
            {
                advanceChanged = date.DayOfWeek;
                return null;
            }
            var weekly = GerWeeklyJikanwari(className, rawData);

            var dayOfWeek = Schedule.AdvanceChanged(date);
            advanceChanged = dayOfWeek;

            try
            {
                return dayOfWeek switch
                {
                    DayOfWeek.Monday => weekly[0],
                    DayOfWeek.Tuesday => weekly[1],
                    DayOfWeek.Wednesday => weekly[2],
                    DayOfWeek.Thursday => weekly[3],
                    DayOfWeek.Friday => weekly[4],
                    DayOfWeek.Sunday => throw new ArgumentOutOfRangeException(),
                    DayOfWeek.Saturday => throw new ArgumentOutOfRangeException(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return $"{TimeString}\n{Time}講 - {Subject} ({Teacher}) / {Other}";
        }
    }
}