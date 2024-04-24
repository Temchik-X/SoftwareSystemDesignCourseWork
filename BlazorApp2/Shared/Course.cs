using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp2.Shared
{
    public class Course
    {
        public int id { get; set; }
        public int user_id { get; set; }

        public string? course_name { get; set; }
        public bool is_finished { get; set; }
    }
}
