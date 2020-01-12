using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using System.ComponentModel.DataAnnotations;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class EarningModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int EarnCost { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public Branch Branch { get; set; }
        [Required]
        public int BranchId { get; set; }
        public ICollection<Earning> Earnings { get; set; }
        public ICollection<Branch> Branches { get; set; }
    }
}
