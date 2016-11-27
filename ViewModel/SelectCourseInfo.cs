using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 选课系统.ViewModel
{
    public class SelectCourseInfo
    {
        public int AllCoursesNum { get; set; }
        public int SuccessedNum { get; set; }
        public int FailedNum { get; set; }
        public IList<string> FullCourses { get; set; } 
        public IList<string> RepeatCourses { get; set; }
        public IList<string> ContralicCourses { get; set; }
    }
}