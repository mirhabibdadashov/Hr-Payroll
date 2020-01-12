using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Admin.Models.Data.Models
{
    public class Absent
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public PermissionRole PermissionRole { get; set; } = 0;
        [Required]
        public int TotalDay
        {
            get
            {
                return To.Day - From.Day;
            }
        }
        [Required]
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}
