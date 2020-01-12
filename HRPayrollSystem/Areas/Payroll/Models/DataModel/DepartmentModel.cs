using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class DepartmentModel
    {
        public ICollection<Department> Departments { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
