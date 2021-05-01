using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Secure_Website.Models
{
    //https://medium.com/streamwriter/generating-random-passwords-in-asp-net-core-c24449f7c877
    public class StudentModel
    {
        [Key]
        public string StudentId { get; set; }
        
        [StringLength(50, MinimumLength = 5)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50, MinimumLength = 5)]
        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        public virtual ApplicationUser Teacher { get; set; }//this is the relationship

        [ForeignKey("ApplicationUser")]
        public string TeacherId { get; set; }//this is the actual foreign key; this is a way how to address the relationship


    }
}
