using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.Data.Payrole
{
    public class Earning
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
    }
}
