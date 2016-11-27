using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 选课系统.Model
{
    public class Student
    {
        public int ID { get; set; }

        public string UserID { get; set; }
        
        
        public string Major { get; set; }
        
        public string Mentor { get; set; }

        public string PersonInfo { get; set; }
        

    }
}
