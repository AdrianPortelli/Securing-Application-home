using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Secure_Website.Models
{
    public class RolesManagementModel
    {
      public List<IdentityRole> Roles { get; set; }

      public List<ApplicationUser> Users { get; set; }
    }
}
