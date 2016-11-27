using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using 选课系统.Model;

namespace 选课系统.ViewModel
{
    public class CourseListVM:ViewModelBase
    {
        public IEnumerable<Course> CourseList { get; set; }
    }
}