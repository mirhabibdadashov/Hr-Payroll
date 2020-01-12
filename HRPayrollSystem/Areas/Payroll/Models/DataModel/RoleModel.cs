using HRPayrollSystem.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class RoleModel
    {
        public List<Employee> Employees { get; set; }
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public List<IdentityRole> Roles { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
