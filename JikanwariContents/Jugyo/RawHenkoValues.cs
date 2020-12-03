using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace SnctJikanwari.JikanwariContents.Jugyo
{
    public class RawHenkoValues
    {
        public IElement StatusAndDate;
        public string TimetableNumber;
        public string ClassName;
        public string Subject;
        public string Teacher;
        public string Others;

        private RawHenkoValues()
        {
            StatusAndDate = null!;
            TimetableNumber = string.Empty;
            ClassName = string.Empty;
            Subject = string.Empty;
            Teacher = string.Empty;
            Others = string.Empty;
        }

        public static IEnumerable<RawHenkoValues> GetParsedHtml()
        {
            const string urlString = "https://www.sendai-nct.ac.jp/sclife/kyuko/ku_hirose/";
            var request = WebRequest.Create(urlString);
            request.Timeout = 10000;
            using var response = request.GetResponse();
            using var source = response.GetResponseStream();

            var parser = new HtmlParser();
            var doc = parser.ParseDocument(source);

            // ReSharper disable once StringLiteralTypo
            var items = doc.QuerySelectorAll("#kuinfo > tbody > tr")
                .SelectMany(item =>
                {
                    var data = item.GetElementsByTagName("td");

                    var statusAndDate = data[0];
                    var className = data[1].TextContent;
                    var rawTime = data[2].TextContent;
                    var time = string.IsNullOrEmpty(rawTime) ? "" : rawTime;
                    var timeRe = new Regex("[1-9]校時|[1-9],");
                    var timeMatch = timeRe.Matches(time);
                    var numRe = new Regex("[0-9]");
                    var matchNumbers = timeMatch.Select(m => numRe.Match(m.Value).Value);
                    var subject = data[3].TextContent;
                    var teacher = data[4].TextContent;
                    var others = data[5].TextContent;

                    return matchNumbers.Select(t => new RawHenkoValues
                    {
                        StatusAndDate = statusAndDate,
                        TimetableNumber = t,
                        ClassName = className,
                        Subject = subject,
                        Teacher = teacher,
                        Others = others
                    });
                });

            return items;
        }
    }
}