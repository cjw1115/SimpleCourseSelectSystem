using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 选课系统.Attribute;

namespace 选课系统.Model
{
    public class User
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Contact { get; set; }

        [Key]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserType UserType { get; set; }

        [Ignore]
        public Student Student { get; set; }
        
    }

    public enum UserType
    {
        Student=1,
        Teacher
    }
}
