using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.Data.Payrole
{
    public class BranchPosition
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public Branch Branch { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public Position Position { get; set; }
        [Required]
        public int PositionId { get; set; }
    }
}
