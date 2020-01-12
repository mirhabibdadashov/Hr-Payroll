using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace HRPayrollSystem.Areas.Admin.Models.DataModel
{
    public class OldWorkModel
    {
        public int Id { get; set; }
        [Required]
        public string HoldingName { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string BranchName { get; set; }
        [Required]
        public string DepartmentName { get; set; }
        [Required]
        public string PositionName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EnterTime { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime LeaveTime { get; set; }
        [Required]
        public string Reason { get; set; }
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}