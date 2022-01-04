﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeCutter.Models;

namespace YoutubeCutter.Controls
{
    public class ClipItem
    {
        public string Filename { get; set; }
        public Time StartTime { get; set; }
        public Time EndTime { get; set; }
        public bool IsCorrectClip { get; set; }
    }
}
