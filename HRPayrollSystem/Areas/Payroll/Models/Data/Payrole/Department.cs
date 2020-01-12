using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace  HRPayrollSystem.Areas.Admin.Models
{
    public class Department
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Position> Positions { get; set; }
    }
}