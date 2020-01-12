using HRPayrollSystem.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class BonusModel
    {
        public ICollection<Bonus> Bonuses { get; set; }
        public Employee Employee { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime GivenDate { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}
