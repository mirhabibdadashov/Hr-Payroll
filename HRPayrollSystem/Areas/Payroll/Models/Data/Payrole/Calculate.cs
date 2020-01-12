using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.Data.Payrole
{
    public class Calculate
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime CalculateDate { get; set; }
        [Required]
        public decimal TotalSalary { get; set; }
        [Required]
        public int TotalAbsentDays { get; set; }
        [Required]
        public decimal TotalPenalties { get; set; }
        [Required]
        public decimal TotalBonuses { get; set; }
        [Required]
        public decimal TotalVacationDays { get; set; }
        [Required]
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}
