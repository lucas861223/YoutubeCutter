using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCutter.Models
{
    public class Time
    {
        public int Hour { set; get; }
        public int Second { set; get; }
        public int Minute { set; get; }

        public static Time operator -(Time x, Time y)
        {
            return new Time() { Hour = x.Hour - y.Hour, Minute = x.Minute - y.Minute, Second = x.Second - y.Second };
        }
    }
}
