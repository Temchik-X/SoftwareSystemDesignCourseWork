using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp2.Shared
{
    public class HrProcess
    {
        [Key]
        public int id { get; set; }
        public string? vacancy { get; set; }

        public string? status { get; set; }
    }
}
