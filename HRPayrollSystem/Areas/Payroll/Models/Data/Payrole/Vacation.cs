using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.Data.Payrole
{
    public class Vacation
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}
