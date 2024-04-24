using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp2.Shared
{
    public class Employee
    {
        public int id { get; set; }
        public string? name { get; set; }

        public string? jobtitle { get; set; }
        public string? competencies { get; set; }
    }
}
