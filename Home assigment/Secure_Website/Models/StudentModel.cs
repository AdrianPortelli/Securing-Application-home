using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Secure_Website.Models
{
    //https://medium.com/streamwriter/generating-random-passwords-in-asp-net-core-c24449f7c877
    public class StudentModel
    {
        [StringLength(50, MinimumLength = 5)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 5)]
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
