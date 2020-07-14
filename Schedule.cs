using System;
using System.Collections.Generic;
using System.Linq;

namespace SnctJikanwari
{
    public class Schedule
    {
        private DateTime StDate { get; }
        private DateTime EnDate { get; }
        private string? AllGrade { get; }
        private string? FirstGrade { get; }
        private string? SecondGrade { get; }
        private string? ThirdGrade { get; }
        private string? FourthGrade { get; }
        private string? FifthGrade { get; }

        private string?[] GradeSchedules => new[]
            {AllGrade, FirstGrade, SecondGrade, ThirdGrade, FourthGrade, FifthGrade};

        private DayOfWeek? ChangeJugyoWeek { get; }


        private Schedule(DateTime stDate, string allGrade = "", string first = "", string second = "",
            string third = "", string fourth = "", string fifth = "", DateTime? enDate = null, DayOfWeek? chWeek = null)
        {
            StDate = stDate;
            EnDate = enDate ?? StDate;
            AllGrade = string.IsNullOrEmpty(allGrade) ? null : allGrade;
            FirstGrade = string.IsNullOrEmpty(first) ? null : first;
            SecondGrade = string.IsNullOrEmpty(second) ? null : second;
            ThirdGrade = string.IsNullOrEmpty(third) ? null : third;
            FourthGrade = string.IsNullOrEmpty(fourth) ? null : fourth;
            FifthGrade = string.IsNullOrEmpty(fifth) ? null : fifth;
            ChangeJugyoWeek = chWeek;
        }

        private static IEnumerable<Schedule> GetSchedules()
        {
            yield return new Schedule(new DateTime(2020, 6, 19), "休業日（開寮のため）", second: "開寮", third: "開寮", fourth: "開寮",
                fifth: "開寮");
            yield return new Schedule(new DateTime(2020, 6, 20), first: "開寮");
            yield return new Schedule(new DateTime(2020, 6, 22), "対面授業開始（登校日は時間割参照）", "HR・オリエンテーション");
            yield return new Schedule(new DateTime(2020, 7, 23), "オープンキャンパス（未定）", enDate: new DateTime(2020, 7, 24));
            yield return new Schedule(new DateTime(2020, 8, 8), "閉寮");
            yield return new Schedule(new DateTime(2020, 8, 11), "夏季休暇", enDate: new DateTime(2020, 8, 23));
            yield return new Schedule(new DateTime(2020, 8, 24), "授業再開");
            yield return new Schedule(new DateTime(2020, 9, 23), "前期期末試験 前半", enDate: new DateTime(2020, 9, 25));
            yield return new Schedule(new DateTime(2020, 9, 28), "前期期末試験 後半", enDate: new DateTime(2020, 9, 30));
            yield return new Schedule(new DateTime(2020, 10, 1), "開校記念日（学生休業日）");
            yield return new Schedule(new DateTime(2020, 10, 2), "後期授業開始");
            yield return new Schedule(new DateTime(2020, 10, 10), "プロコン本選（オンライン）", enDate: new DateTime(2020, 10, 11));
            yield return new Schedule(new DateTime(2020, 10, 18), "ロボコン東北地区大会（オンライン）");
            yield return new Schedule(new DateTime(2020, 10, 23), "地区大会（ラグビー・鶴岡）", enDate: new DateTime(2020, 10, 27));
            yield return new Schedule(new DateTime(2020, 10, 24), "学生健康診断");
            yield return new Schedule(new DateTime(2020, 10, 30), "1 授業、2~ 高専祭準備");
            yield return new Schedule(new DateTime(2020, 10, 31), "高専祭", enDate: new DateTime(2020, 11, 1));
            yield return new Schedule(new DateTime(2020, 11, 2), "学生休業日（高専祭振替）");
            yield return new Schedule(new DateTime(2020, 11, 4), "月曜授業", chWeek: DayOfWeek.Monday);
            yield return new Schedule(new DateTime(2020, 11, 10), fourth: "研修旅行", enDate: new DateTime(2020, 11, 13));
            yield return new Schedule(new DateTime(2020, 11, 14), fourth: "予備日（研修旅行）");
            yield return new Schedule(new DateTime(2020, 11, 16), "学生会立会演説会（4校）");
            yield return new Schedule(new DateTime(2020, 11, 29), "ロボコン全国大会");
            yield return new Schedule(new DateTime(2020, 12, 1), first: "後期中間試験", second: "後期中間試験", third: "後期中間試験",
                enDate: new DateTime(2020, 12, 3));
            yield return new Schedule(new DateTime(2020, 12, 5), "デザコン全国大会", enDate: new DateTime(2020, 12, 6));
            yield return new Schedule(new DateTime(2020, 12, 20), "吹奏楽部定期演奏会");
            yield return new Schedule(new DateTime(2020, 12, 23), "~3 授業 4~ 大掃除・HR");
            yield return new Schedule(new DateTime(2020, 12, 24), "冬季休業", enDate: new DateTime(2021, 1, 5));
            yield return new Schedule(new DateTime(2020, 12, 24), "閉寮");
            yield return new Schedule(new DateTime(2021, 1, 4), "全国高専大会（ラグビー）", enDate: new DateTime(2021, 1, 9));
            yield return new Schedule(new DateTime(2021, 1, 5), "開寮");
            yield return new Schedule(new DateTime(2021, 1, 6), "授業再開");
            yield return new Schedule(new DateTime(2021, 1, 13), "推薦入試（学生登校禁止）");
            yield return new Schedule(new DateTime(2021, 1, 14), "月曜授業", chWeek: DayOfWeek.Monday);
            yield return new Schedule(new DateTime(2021, 1, 25), fifth: "後期期末試験", enDate: new DateTime(2021, 1, 28));
            yield return new Schedule(new DateTime(2021, 1, 30), "全国高専英語プレコン（オリセン）", enDate: new DateTime(2021, 1, 31));
            yield return new Schedule(new DateTime(2021, 2, 5), first: "後期期末試験 #1", second: "後期期末試験 #1",
                third: "後期期末試験 #1", fourth: "後期期末試験 #1", fifth: "補講期間");
            yield return new Schedule(new DateTime(2021, 2, 8), first: "後期期末試験 #2", second: "後期期末試験 #2",
                third: "後期期末試験 #2", fourth: "後期期末試験 #2", fifth: "補講期間", enDate: new DateTime(2021, 2, 10));
            yield return new Schedule(new DateTime(2021, 2, 12), first: "後期期末試験 #3", second: "後期期末試験 #3",
                third: "後期期末試験 #3", fourth: "後期期末試験 #3", fifth: "補講期間");
            yield return new Schedule(new DateTime(2021, 2, 15), "補講期間", enDate: new DateTime(2021, 2, 17));
            yield return new Schedule(new DateTime(2021, 2, 17), "終業式・予餞会（3校時）");
            yield return new Schedule(new DateTime(2021, 2, 18), first: "補講期間", second: "補講期間", third: "補講期間",
                fourth: "補講期間 #3", fifth: "卒論締め切り");
            yield return new Schedule(new DateTime(2021, 2, 21), "学力入試（学生登校禁止）");
            yield return new Schedule(new DateTime(2021, 2, 22), "学力入試採点日（学生休業日）");
            yield return new Schedule(new DateTime(2021, 2, 23), "学力入試予備試験日");
            yield return new Schedule(new DateTime(2021, 2, 24), fifth: "卒研発表", enDate: new DateTime(2021, 2, 25));
            yield return new Schedule(new DateTime(2021, 2, 26), "学力入試合格発表");
            yield return new Schedule(new DateTime(2021, 3, 4), "入学説明会・合格者面接・入寮説明会");
            yield return new Schedule(new DateTime(2021, 3, 6), "閉寮");
            yield return new Schedule(new DateTime(2021, 3, 20), "卒業式・修了式");
        }

        public static IEnumerable<Schedule> GetTodaySchedules(int grade, DateTime? date = null)
        {
            date ??= DateTime.Today;

            return GetSchedules().Where(s => s.StDate <= date && date <= s.EnDate)
                .Where(s => !string.IsNullOrEmpty(s.AllGrade) || !string.IsNullOrEmpty(s.GradeSchedules[grade]));
        }

        public static IEnumerable<Schedule> GetGradeFutureSchedules(int grade, DateTime? stDate = null)
        {
            stDate ??= DateTime.Today;
            return GetSchedules().SkipWhile(s => s.EnDate < stDate)
                .Where(s => !string.IsNullOrEmpty(s.AllGrade) || !string.IsNullOrEmpty(s.GradeSchedules[grade]));
        }

        public static IEnumerable<Schedule> GetFutureSchedules(DateTime? stDate = null)
        {
            stDate ??= DateTime.Today;
            return GetSchedules().SkipWhile(s => s.EnDate < stDate);
        }

        public static DayOfWeek AdvanceChanged(DateTime date)
        {
            var dateSchedules = GetSchedules().Where(s => s.StDate <= date && date <= s.EnDate);
            var schedules = dateSchedules.ToList();
            if (!schedules.Any()) return date.DayOfWeek;
            foreach (var schedule in schedules.Where(schedule => schedule.ChangeJugyoWeek != null))
            {
                return schedule.ChangeJugyoWeek!.Value;
            }

            return date.DayOfWeek;
        }

        public override string ToString()
        {
            return $"{StDate:MM/dd}{(StDate == EnDate ? "" : $"-{EnDate:MM/dd}")}" +
                   $"{(string.IsNullOrEmpty(AllGrade) ? "" : $"\n{AllGrade} (全学年)")}" +
                   $"{(string.IsNullOrEmpty(FirstGrade) ? "" : $"\n{FirstGrade} (1年)")}" +
                   $"{(string.IsNullOrEmpty(SecondGrade) ? "" : $"\n{SecondGrade} (2年)")}" +
                   $"{(string.IsNullOrEmpty(ThirdGrade) ? "" : $"\n{ThirdGrade} (3年)")}" +
                   $"{(string.IsNullOrEmpty(FourthGrade) ? "" : $"\n{FourthGrade} (4年)")}" +
                   $"{(string.IsNullOrEmpty(FifthGrade) ? "" : $"\n{FifthGrade} (5年)")}";
        }

        private string? GetGradeSchedule(int grade)
        {
            return grade switch
            {
                1 => FirstGrade,
                2 => SecondGrade,
                3 => ThirdGrade,
                4 => FourthGrade,
                5 => FifthGrade,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string ToString(int grade)
        {
            if (grade == 0) return ToString();
            var gradeSchedule = GetGradeSchedule(grade);

            var eol = !string.IsNullOrEmpty(AllGrade) && !string.IsNullOrEmpty(gradeSchedule) ? "\n" : "";

            return $"{StDate:MM/dd}{(StDate == EnDate ? "" : $"-{EnDate:MM/dd}")}\n" +
                   $"{(string.IsNullOrEmpty(AllGrade) ? "" : $"{AllGrade}{eol}")}" +
                   $"{(string.IsNullOrEmpty(gradeSchedule) ? "" : gradeSchedule)}";
        }

        public string SimpleText(int grade)
        {
            var gradeSchedule = GetGradeSchedule(grade);
            var eol = !string.IsNullOrEmpty(AllGrade) && !string.IsNullOrEmpty(gradeSchedule) ? "\n" : "";

            return
                $"{(string.IsNullOrEmpty(AllGrade) ? "" : $"{AllGrade}{eol}")}" +
                $"{(string.IsNullOrEmpty(gradeSchedule) ? "" : gradeSchedule)}";
        }
    }
}