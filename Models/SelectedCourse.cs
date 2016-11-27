using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 选课系统.Model
{
    public class SelectedCourse
    {
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public float Score { get; set; }

        public int Credit { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string CourseName { get; set; }
        public string Mentor { get; set; }

    }
}
