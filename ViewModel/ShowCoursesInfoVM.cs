using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 选课系统.ViewModel
{
    public class ShowCoursesInfoVM:ViewModelBase
    {
        public IEnumerable<Model.Course> Couses { get; set; }
        public object Param { get; set; }
    }
}