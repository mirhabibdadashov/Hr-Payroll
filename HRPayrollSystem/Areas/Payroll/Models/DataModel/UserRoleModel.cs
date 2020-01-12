using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class UserRoleModel
    {
        public IdentityUser User { get; set; }
        public IdentityRole Role { get; set; }
        public ICollection<IdentityRole> Roles { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
