using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace 选课系统.ViewModel
{
    public class GetChartVM 
    {
        public IEnumerable<Model.SelectedCourse> Courses { get; set; }
        public Model.User User { get; set; }
    }
}