using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace  HRPayrollSystem.Areas.Admin.Models
{
    public class Branch
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Company Company { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public ICollection<Work> Works { get; set; }
        public ICollection<BranchPosition> BranchPositions { get; set; }
        public ICollection<Grade> Grades { get; set; }
    }
}