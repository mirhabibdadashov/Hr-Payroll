using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace  HRPayrollSystem.Areas.Admin.Models
{
    public class Company
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Holding Holding { get; set; }
        [Required]
        public int HoldingId { get; set; }
        public ICollection<Branch> Branches { get; set; }
    }
}