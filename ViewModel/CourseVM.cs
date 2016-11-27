using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace 选课系统.ViewModel
{
    public class CourseVM
    {
        
        public int CourseID { get; set; }

        public string 选课方向 { get; set; }

        public string CourseName { get; set; }
        public string Mentor { get; set; }
        public string Cstyle { get; set; }
        public double Credit { get; set; }
        public string time { get; set; }
        public string jieci { get; set; }
        public string StartClass { get; set; }
        public string EndClass { get; set; }
        public string AllWeek { get; set; }
        public string StartWeek { get; set; }
        public string EndWeek { get; set; }
        public int xueshi { get; set; }
        public int MaxNum { get; set; }
        public int RealNum { get; set; }
        public DateTime Cstart { get; set; }
        public DateTime Cend { get; set; }
        public string alltime { get; set; }
        public int Flag { get; set; }
    }
}