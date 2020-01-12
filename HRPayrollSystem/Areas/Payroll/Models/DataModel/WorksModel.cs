using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Admin.Models.DataModel
{
    public class WorksModel
    {
        public List<OldWork> OldWorks { get; set; }
        public List<Work> Works { get; set; }
        public Employee Employee { get; set; }
    }
}
