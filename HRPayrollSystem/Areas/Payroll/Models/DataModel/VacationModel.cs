using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class VacationModel
    {

        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public ICollection<Vacation> Vacations { get; set; }
    }
}
