using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.Data.Payrole
{
    public class Grade
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int From { get; set; }
        [Required]
        public int To { get; set; }
        [Required]
        public decimal Bonus { get; set; }
        [Required]
        public Cost Cost { get; set; }
        [Required]
        public Branch Branch { get; set; }
        [Required]
        public int BranchId { get; set; }
    }
}
