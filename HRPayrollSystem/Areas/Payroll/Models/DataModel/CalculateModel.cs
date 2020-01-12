using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Payroll.Models.DataModel
{
    public class CalculateModel
    {
        public IEnumerable<Calculate> Calculates { get; set; }
        public int Month { get; set; }
    }
}
