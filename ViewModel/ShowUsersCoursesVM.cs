using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 选课系统.ViewModel
{
    public class ShowUsersCoursesVM:ViewModelBase
    {
        public IEnumerable<选课系统.Model.SelectedCourse> SelectedCourses { get; set; }
        public Model.SelectedCourse Param { get; set; }
    }
}