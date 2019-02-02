using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ifs9webapp
{
    public class RequestViewModel
    {
        [Required]
        [Display(Name = "RedirectToIfs")]
        public bool RedirectToIfs { get; set; }
        [Required]
        [Display(Name = "id")]
        public bool id { get; set; }

    }
}
