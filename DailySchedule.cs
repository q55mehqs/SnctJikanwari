using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using LineApi;
using LineApi.ResponseObjects.MessageObject;
using SnctJikanwari.JikanwariContents;

namespace SnctJikanwari
{
    public static class DailySchedule
    {
        private static readonly AmazonDynamoDBConfig Config = new AmazonDynamoDBConfig
            {RegionEndpoint = RegionEndpoint.USEast2};

        private static readonly AmazonDynamoDBClient Client = new AmazonDynamoDBClient(Config);

        private static async Task<ScanResponse> GetHourUsers()
        {
            var now = DateTime.Now;

            var req = new ScanRequest
            {
                TableName = "LineUsers",
                FilterExpression = "#dmh = :h",
                ExpressionAttributeNames = new Dictionary<string, string> {{"#dmh", "DailyMessageHour"}},
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {{":h", new AttributeValue {N = now.Hour.ToString()}}}
            };

            return await Client.ScanAsync(req);
        }

        private static async Task<QueryResponse> GetKadai(string className)
        {
            var now = DateTime.Now;

            // TODO 要検証
            var limitDate = DateTime.Today.AddDays(1).SkipHoliday();

            var req = new QueryRequest
            {
                TableName = "Kadai",
                KeyConditionExpression = "#cn = :c AND #dc BETWEEN :now AND :lim",
                ExpressionAttributeNames = new Dictionary<string, string>
                    {{"#cn", "Class"}, {"#dc", "DeadlineCreatedUserId"}},
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":c", new AttributeValue {S = className}},
                    {":now", new AttributeValue {S = now.ToTimeStampString()}},
                    {":lim", new AttributeValue {S = limitDate.ToTimeStampString()}}
                }
            };

            return await Client.QueryAsync(req);
        }

        private static string ReplaceEofToSpace(string source)
        {
            var re = new Regex(@"(\n|\r\n)");
            return re.Replace(source, " ");
        }

        private static async Task<string> MakeKadaiTextTask(string className)
        {
            var userKadaiResponse = await GetKadai(className);
            if (userKadaiResponse.Count == 0) return "クラスに登録されている課題なし";

            CultureInfo.CurrentCulture = new CultureInfo("ja-JP", false);
            var items = userKadaiResponse.Items;
            var formatItems = items
                .Select(k => new
                {
                    Subject = k["Subject"].S,
                    Info = ReplaceEofToSpace(k["Information"].S),
                    Deadline = k["DeadlineTime"].N.ToLocalDateTime()
                })
                .Select(k => $"{k.Subject}: {k.Info} (~{k.Deadline:HH:mm})");

            return string.Join("\n", formatItems);
        }

        private static async Task<string> MakeJikanwariTextTask(string className)
        {
            var date = DateTime.Now.Hour < 16 ? DateTime.Today : DateTime.Today.AddDays(1);
            date = date.SkipHoliday();

            var (classJikanwari, _) = await JikanwariManager.GetJikanwari(className, date);
            return JikanwariManager.JikanwariText(classJikanwari, className.Contains("AS"));
        }

        private static string MakeScheduleTextTask(string className)
        {
            var grade = GetGrade(className);
            var date = DateTime.Now.Hour < 16 ? DateTime.Today : DateTime.Today.AddDays(1);
            date = date.SkipHoliday();

            var gradeSchedule = Schedule.GetTodaySchedules(grade, date).ToList();
            return gradeSchedule.Count == 0
                ? "予定なし"
                : string.Join("\n", gradeSchedule.Select(s => s.SimpleText(grade)));
        }

        private static async Task<string> GetClassMessage(string className)
        {
            var date = DateTime.Now.Hour < 16 ? DateTime.Today : DateTime.Today.AddDays(1);
            date = date.SkipHoliday();

            var kadaiTask = MakeKadaiTextTask(className);
            var jikanwariTask = MakeJikanwariTextTask(className);
            var schedule = MakeScheduleTextTask(className);

            CultureInfo.CurrentCulture = new CultureInfo("ja-JP", false);
            return $"{date:MM/dd(ddd)} 時間割\n{await jikanwariTask}\n\n行事予定\n{schedule}\n\n" +
                   $"{date:MM/dd} 提出の{className}課題\n{await kadaiTask}";
        }

        private static int GetGrade(string className)
        {
            var firstRe = new Regex(@"1[年-]");
            var firstM = firstRe.Match(className);
            if (firstM.Success) return 1;

            var otherRe = new Regex(@"[AI][STE][2-5]");
            var otherM = otherRe.Match(className);
            if (!otherM.Success) throw new Exception();

            var numRe = new Regex(@"\d");
            var numMatch = numRe.Match(otherM.Value);
            return int.Parse(numMatch.Value);
        }

        public static async Task DailyScheduleTask()
        {
            var hourUsersResponse = await GetHourUsers();
            var hourUsers = hourUsersResponse.Items;

            var dic = new Dictionary<string, List<string>>();
            foreach (var user in hourUsers.Where(u => u.ContainsKey("Class"))
                .Select(u => new
                {
                    UserId = u["UserId"].S,
                    UserClasses = u["Class"].SS
                }))
            {
                foreach (var userClass in user.UserClasses)
                {
                    if (!dic.ContainsKey(userClass))
                    {
                        dic.Add(userClass, new List<string>());
                    }

                    dic[userClass].Add(user.UserId);
                }
            }

            var classMessages = dic.Keys.ToDictionary(cln => cln, GetClassMessage);

            var tasks = new List<Task>();
            foreach (var (className, users) in dic)
            {
                var message = await classMessages[className];
                var lineMessage = new[] {new TextMessage(message)};
                if (users.Any(u => u.StartsWith("U")))
                {
                    tasks.Add(LineManager.Multicast(users.Where(u => u.StartsWith("U")), lineMessage));
                }

                tasks.AddRange(users.Where(u => u.StartsWith("C")).Select(u => LineManager.Push(u, lineMessage)));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public static async Task<TextMessage> GetMessage(string className)
        {
            return new TextMessage(await GetClassMessage(className));
        }
    }
}