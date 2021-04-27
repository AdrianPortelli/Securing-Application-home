using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Secure_Website.Models
{
    //https://medium.com/streamwriter/generating-random-passwords-in-asp-net-core-c24449f7c877
    public class StudentModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
