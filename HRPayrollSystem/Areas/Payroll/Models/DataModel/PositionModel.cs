using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class PositionModel
    {
        public ICollection<Department> Departments { get; set; }
        public ICollection<Position> Positions { get; set; }
        public ICollection<Salary> Salaries { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int SalaryId { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }
}
