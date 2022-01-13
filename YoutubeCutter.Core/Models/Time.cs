using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCutter.Core.Models
{
    public class Time
    {
        public int Hour { set; get; }
        public int Second { set; get; }
        public int Minute { set; get; }

        public static Time operator -(Time x, Time y)
        {
            Time time = new Time();
            time.Second = x.Second - y.Second;
            time.Minute = x.Minute - y.Minute;
            time.Hour = x.Hour - y.Hour;
            if (time.Second < 0 && (time.Minute > 0 || time.Hour > 0))
            {
                time.Second += 60;
                time.Minute -= 1;
            }
            if (time.Minute < 0 && time.Hour > 0)
            {
                time.Minute += 60;
                time.Hour -= 1;
            }
            return time;
        }
    }
}
