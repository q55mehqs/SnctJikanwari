using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SnctJikanwari.JikanwariContents.Jugyo
{
    public class HenkoJugyo : IJugyo
    {
        private string HenkoStatus { get; }
        private DateTime Date { get; }
        public int Time { get; }
        public string Subject { get; }
        public string ClassName { get; }
        public string Teacher { get; }
        public string Other { get; }

        private HenkoJugyo(RawHenkoValues henkoValues)
        {
            HenkoStatus = henkoValues.StatusAndDate.Children[0].TextContent;
            // ex) values.StatusAndDate.TextContent = "変更12月4日(水)"
            //     strDate = "12月4日"
            var strDate = henkoValues.StatusAndDate.TextContent.Substring(2,
                henkoValues.StatusAndDate.TextContent.IndexOf("(", StringComparison.CurrentCulture) - 2);
            Date = DateTime.ParseExact(strDate, "M月d日", null);
            if (DateTime.Today.Month > 4 && Date.Month <= 3)
            {
                Date = Date.AddYears(1);
            }

            Time = int.Parse(henkoValues.RawTime.Substring(0, 1));
            Subject = henkoValues.Subject;
            ClassName = henkoValues.ClassName;
            Teacher = henkoValues.Teacher == "" ? "__" : henkoValues.Teacher;
            Other = henkoValues.Others == "" ? "__" : henkoValues.Others;
        }

        public static List<HenkoJugyo> GetAllClassHenko()
        {
            var henkos = new List<HenkoJugyo>();
            var rawHenkos = RawHenkoValues.GetParsedHtml();

            henkos.AddRange(rawHenkos.Select(rawHenko => new HenkoJugyo(rawHenko)));

            return henkos;
        }

        public static List<HenkoJugyo> GetClassHenko(string className, List<HenkoJugyo>? allClassHenkos = null)
        {
            allClassHenkos ??= GetAllClassHenko();
            var classHenkos = new List<HenkoJugyo>();
            classHenkos.AddRange(allClassHenkos.FindAll(henko => henko.ClassName.Contains(className)));

            return classHenkos;
        }

        public static List<HenkoJugyo> GetDailyHenko(DateTime date)
        {
            var henkos = GetAllClassHenko();
            return henkos.FindAll(h => h.Date == date);
        }

        public static IEnumerable<HenkoJugyo> GetDailyClassHenko(string className, DateTime date,
            List<HenkoJugyo>? classHenkos = null, List<HenkoJugyo>? allClassHenkos = null)
        {
            classHenkos ??= GetClassHenko(className, allClassHenkos);
            var classDailyHenkos = new List<HenkoJugyo>();
            classDailyHenkos.AddRange(classHenkos.FindAll(h => h.Date == date));

            return classDailyHenkos;
        }

        public override string ToString()
        {
            return $"{Time}講 - ({HenkoStatus}) {Subject} ({Teacher}) / {Other}";
        }

        public string ToString(bool hasClassName)
        {
            var text = $"{Time}講 - ({HenkoStatus}) {Subject} ({Teacher}) / {Other}";
            if (hasClassName)
            {
                text += $" ({ClassName})";
            }

            return text;
        }

        public string HenkoString()
        {
            return $"{HenkoStatus} - {Date.ToString("MM/dd (ddd)", new CultureInfo("ja-JP"))} " +
                   $"{Time}講 {Subject}\n...あと{(Date - DateTime.Today).Days}日";
        }
    }
}