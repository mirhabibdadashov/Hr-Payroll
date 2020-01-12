using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using System;
using System.ComponentModel.DataAnnotations;

namespace  HRPayrollSystem.Areas.Admin.Models
{
    public class Work
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public BranchPosition BranchPosition { get; set; }
        [Required]
        public int BranchPositionId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EnterTime { get; set; }
        [DataType(DataType.Date)]
        public DateTime? LeaveTime { get; set; }
        public string Reason { get; set; }
        [Required]
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}