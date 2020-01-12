using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace  HRPayrollSystem.Areas.Admin.Models
{
    public class Fine
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime GivenDate { get; set; }
        [Required]
        public Employee Employee { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}
