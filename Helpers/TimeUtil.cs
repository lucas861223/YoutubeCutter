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
            //it's probably faster to just do dummy check
            int x;
            if (text.Length != 8)
            {
                return false;
            }
            if (!int.TryParse(text.Substring(0, 2), out x) || x >= 60) {
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
    }
}
