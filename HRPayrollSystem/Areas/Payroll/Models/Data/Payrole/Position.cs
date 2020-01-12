using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace  HRPayrollSystem.Areas.Admin.Models
{
    public class Position
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Salary Salary { get; set; }
        [Required]
        public int SalaryId { get; set; }
        [Required]
        public Department Department { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public ICollection<Work> Works { get; set; }
        public ICollection<BranchPosition> BranchPositions { get; set; }
    }
}