using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class PayrollModel
    {
        public ICollection<Work> Works { get; set; }
        public ICollection<Absent> Absents { get; set; }
        public decimal Salary { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalPenalties { get; set; }
        public int TotalAbsentDay { get; set; }
        public decimal TotalAbsentPenalty { get; set; }
        public decimal TotalGrade { get; set; }
        public decimal TotalVacation { get; set; }
        public decimal TotalSalary { get; set; }
        public Work Work { get; set; }
    }
}
