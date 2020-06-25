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
        public static async Task<(List<IJugyo>, DayOfWeek)> GetJikanwari(string className, DateTime date, string cache = "",
            bool useFuture = false)
        {
            var rawJikanwariData =
                string.IsNullOrEmpty(cache) ? await LoadJikanwari(className, useFuture) : cache;
            var dailyJikanwari = DefaultJugyo.GetDailyJikanwari(className, date, rawJikanwariData, out var dayOfWeek)?
                .Cast<IJugyo>().ToList();

            var isDefaultEmpty = false;
            if (dailyJikanwari == null)
            {
                isDefaultEmpty = true;
                dailyJikanwari = new List<IJugyo>();
            }

            var dailyHenko = HenkoJugyo.GetDailyClassHenko(className, date);

            foreach (var henko in dailyHenko)
            {
                if (isDefaultEmpty)
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

        private static async Task<string> GetVersionDirectoryAsync(bool useFuture)
        {
            var assembly = Assembly.GetExecutingAssembly();
            await using var stream =
                assembly.GetManifestResourceStream("SnctJikanwari.DefaultJikanwari.versionStart.csv");
            if (stream == null) throw new Exception("version stream cannot open");
            using var sr = new StreamReader(stream);
            var text = await sr.ReadToEndAsync();
            var lines = text.Split("\n").Select(t => t.Split(","));

            return (useFuture ? lines.Last() : lines.Last(s => DateTime.Parse(s[0]) <= DateTime.Today))[1];
        }

        public static async Task<string> LoadJikanwari(string className, bool useFuture = false)
        {
            var dirName = await GetVersionDirectoryAsync(useFuture);
            var assembly = Assembly.GetExecutingAssembly();
            await using var stream =
                assembly.GetManifestResourceStream($"SnctJikanwari.DefaultJikanwari.{dirName}.{className}.csv");
            if (stream == null) throw new Exception("stream can't get");
            using var sr = new StreamReader(stream);

            var raw = await sr.ReadToEndAsync();
            return raw;
        }

        public static string JikanwariText(IReadOnlyList<IJugyo> jikanwari)
        {
            return jikanwari.Count == 0
                ? "ASクラスの授業はありません"
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