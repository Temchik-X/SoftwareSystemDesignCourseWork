using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp2.Shared
{
    public class DepartmentGoal
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
    }
}
