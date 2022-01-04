using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCutter.Models;
using System.Text.RegularExpressions;

namespace YoutubeCutter.Helpers
{
    class TimeUtil
    {
        private static Regex _durationRegex = new Regex(@"(((?<Hours>\d+):)?(?<Minutes>\d+):)?(?<Seconds>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void ParseTimeFromString(string text, Time time)
        {
            Match match = _durationRegex.Match(text);
            if (match.Success)
            {
                time.Second = int.Parse(match.Groups["Seconds"].ToString());
                time.Minute = match.Groups["Minutes"].ToString() != "" ? int.Parse(match.Groups["Minutes"].ToString()) : 0;
                time.Hour = match.Groups["Hours"].ToString() != "" ? int.Parse(match.Groups["Hours"].ToString()) : 0;
            }
        }

        public static Time CalculateDuration(Time start, Time end)
        {
            return new Time() { Hour = end.Hour - start.Hour, Minute = end.Minute - start.Minute, Second = end.Second - start.Second};
        }

        public static string TimeToString(Time time)
        {
            return String.Format("{0,0:D2}{1,0:D2}{2,0:D2}", time.Hour, time.Minute, time.Second);
        }
    }
}
