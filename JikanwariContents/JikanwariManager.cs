using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SnctJikanwari.JikanwariContents.Jugyo;

namespace SnctJikanwari.JikanwariContents
{
    public static class JikanwariManager
    {
        public static async Task<(List<IJugyo>, DayOfWeek)> GetJikanwari(string className, DateTime date,
            string cache = "")
        {
            var rawJikanwariData =
                string.IsNullOrEmpty(cache) ? await LoadJikanwari(className, date) : cache;
            var dailyJikanwari = DefaultJugyo.GetDailyJikanwari(className, date, rawJikanwariData, out var dayOfWeek)?
                .Cast<IJugyo>().ToList();

            var isDefaultEmpty = false;
            if (dailyJikanwari == null)
            {
                isDefaultEmpty = true;
                dailyJikanwari = new List<IJugyo>();
            }

            var dailyHenko = HenkoJugyo.GetDailyClassHenko(className, date);

            foreach (var henko in dailyHenko.Where(h => !h.Other.Contains("留学")))
            {
                if (isDefaultEmpty || henko.Time - 1 >= dailyJikanwari.Count)
                {
                    dailyJikanwari.Add(henko);
                }
                else
                {
                    dailyJikanwari[henko.Time - 1] = henko;
                }
            }

            return (dailyJikanwari, dayOfWeek);
        }

        private static async Task<string> GetVersionDirectoryAsync(DateTime date)
        {
            var assembly = Assembly.GetExecutingAssembly();
            await using var stream =
                assembly.GetManifestResourceStream("SnctJikanwari.DefaultJikanwari.versionStart.csv");
            if (stream == null) throw new Exception("version steam cannot open");

            using var sr = new StreamReader(stream);
            var text = await sr.ReadToEndAsync();
            var lines = text.Split("\n").Select(t => t.Split(","));

            return lines.Last(s => DateTime.Parse(s[0]) <= date)[1];
        }

        public static async Task<string> LoadJikanwari(string className, DateTime? nullableDate = null)
        {
            var date = nullableDate ?? DateTime.Today;
            var dirName = await GetVersionDirectoryAsync(date);
            var assembly = Assembly.GetExecutingAssembly();
            await using var stream =
                assembly.GetManifestResourceStream($"SnctJikanwari.DefaultJikanwari.{dirName}.{className}.csv");
            if (stream == null) throw new Exception("stream can't get");
            using var sr = new StreamReader(stream);

            var raw = await sr.ReadToEndAsync();
            return raw;
        }

        public static string JikanwariText(IReadOnlyList<IJugyo> jikanwari, bool isAs)
        {
            var noneText = isAs ? "ASクラスの授業はこの曜日には開講されていません 所属クラスの時間割も参照ください" : "該当日に授業は設定されていません";
            return jikanwari.Count == 0
                ? noneText
                : string.Join("\n\n", jikanwari.Select(jugyo => jugyo.ToString()));
        }

        public static string ClassHenkoText(IReadOnlyList<HenkoJugyo> henkos)
        {
            return henkos.Count == 0
                ? "現在このクラスの授業変更はありません"
                : string.Join("\n", henkos.Select(henko => henko.HenkoString()));
        }
    }
}