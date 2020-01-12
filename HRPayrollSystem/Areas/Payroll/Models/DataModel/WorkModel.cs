using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRPayrollSystem.Areas.Admin.Models.DataModel
{
    public class WorkModel
    {
        public int Id { get; set; }
        public ICollection<Holding> Holdings { get; set; }
        public ICollection<BranchPosition> BranchPositions { get; set; }
        public ICollection<Department> Departments { get; set; }
        public BranchPosition BranchPosition { get; set; }
        [Required]
        public int BranchPositionId { get; set; }
        public Position Position { get; set; }
        [Required]
        public int PositionId { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EnterTime { get; set; }
        [DataType(DataType.Date)]
        public DateTime LeaveTime { get; set; } = DateTime.MinValue;
        public string Reason { get; set; }
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}