using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Secure_Website.Models
{
    public class ScheduleTaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(100, MinimumLength = 5)]
        [Required]
        public string TaskName { get; set; }

        [StringLength(250, MinimumLength = 10)]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Deadline")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; }
        public string TeacherId { get; set; }
    }
}
