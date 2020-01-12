using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class SalaryModel
    {
        public ICollection<Salary> Salaries { get; set; }
        [Required]
        public int Price { get; set; }
    }
}
