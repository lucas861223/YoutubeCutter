using System;
using YoutubeCutter.Core.Models;
using System.Text.RegularExpressions;

namespace YoutubeCutter.Core.Helpers
{
    public class TimeUtil
    {
        private static Regex _durationRegex = new Regex(@"(((?<Hours>\d+):)?(?<Minutes>\d+):)?(?<Seconds>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool ParseIrregularTimeFromString(string text, Time time)
        {
            Match match = _durationRegex.Match(text);
            if (match.Success)
            {
                time.Second = int.Parse(match.Groups["Seconds"].ToString());
                time.Minute = match.Groups["Minutes"].ToString() != "" ? int.Parse(match.Groups["Minutes"].ToString()) : 0;
                time.Hour = match.Groups["Hours"].ToString() != "" ? int.Parse(match.Groups["Hours"].ToString()) : 0;
            }
            return match.Success;
        }
        public static Time ParseTimeFromString(string text)
        {
            Time time = new Time();
            ParseTimeFromString(text, time);
            return time;
        }
        public static void ParseTimeFromString(string text, Time time)
        {
            if (IsFormattedTime(text))
            {
                time.Hour = int.Parse(text.Substring(0, 2));
                time.Minute = int.Parse(text.Substring(3, 2));
                time.Second = int.Parse(text.Substring(6, 2));
            }
        }
        public static bool IsFormattedTime(string text)
        {
            //it's probably faster to just do dummy check instead of regex
            int x;
            if (text.Length != 8)
            {
                return false;
            }
            if (!int.TryParse(text.Substring(0, 2), out x)) {
                return false;
            }
            if (text[2] != ':')
            {
                return false;
            }
            if (!int.TryParse(text.Substring(3, 2), out x) || x >= 60)
            {
                return false;
            }
            if (text[5] != ':')
            {
                return false;
            }
            if (!int.TryParse(text.Substring(6, 2), out x))
            {
                return false;
            }
            return x < 60;
        }
        public static Time CalculateDuration(Time start, Time end)
        {
            return new Time() { Hour = end.Hour - start.Hour, Minute = end.Minute - start.Minute, Second = end.Second - start.Second };
        }
        public static string TimeToString(Time time)
        {
            return String.Format("{0,0:D2}:{1,0:D2}:{2,0:D2}", time.Hour, time.Minute, time.Second);
        }
        public static int ConvertToSeconds(Time time)
        {
            return time.Hour * 3600 + time.Minute * 60 + time.Second;
        }
    }
}
