using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Admin.Models.Data.Models
{
    public class OldWork
    {
        [Required]
        public int Id { get; set; }
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
        [Required]
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}
