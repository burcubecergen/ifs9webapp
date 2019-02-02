using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ifs9webapp
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "LoginName")]
        public string LoginName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
