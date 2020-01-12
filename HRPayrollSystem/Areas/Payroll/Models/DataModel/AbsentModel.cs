using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HRPayrollSystem.Areas.Admin.Models.Data.Models;

namespace HRPayrollSystem.Areas.Admin.Models.DataModel
{
    public class AbsentModel
    {
        public List<Employee> Employees { get; set; }
        public List<Absent> Absents { get; set; }
        public List<Absent> AnnualAbsents { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime From { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime To { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public PermissionRole PermissionRole { get; set; } = PermissionRole.Permission;
        public int EmployeeId { get; set; }
        public DateTime LastCheckDate { get; set; }
    }
}