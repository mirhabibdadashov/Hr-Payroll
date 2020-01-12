using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class CompanyModel
    {
        public ICollection<Holding> Holdings { get; set; }
        public ICollection<Company> Companies { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int HoldingId { get; set; }
    }
}
