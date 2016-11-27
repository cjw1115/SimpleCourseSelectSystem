using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace 选课系统.Model
{
    public class Course
    {
        [Key]
        [DataMember(Name ="ID")]
        public int CourseID { get; set; }
        public string Major { get; set; }        

        public string CourseName { get; set; }
        public string Mentor { get; set; }
        public double Credit { get; set; }

        public int StartClass { get; set; }
        public int EndClass { get; set; }
        public string WeekDay { get; set; }

        public int AllWeek { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }

        public int MaxNum { get; set; }
        public int RealNum { get; set; }
        
        public string Room { get; set; }
        public int Flag { get; set; }
    }
}
