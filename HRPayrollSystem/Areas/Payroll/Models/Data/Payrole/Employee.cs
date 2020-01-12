using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace  HRPayrollSystem.Areas.Admin.Models
{
    public class Employee
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string VillageRegister { get; set; }
        [Required]
        public string IdentityNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime IdentityLastDay { get; set; }
        [Required]
        public Education Education { get; set; }
        [Required]
        public FamilyStatus FamilyStatus { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public string PhotoLink { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime LastCalculate { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public IdentityUser IdentityUser { get; set; }
        public string IdentityUserId { get; set; }
        public ICollection<Work> Works { get; set; }
        public ICollection<OldWork> OldWorks { get; set; }
        public ICollection<Fine> Fines { get; set; }
        public ICollection<Bonus> Bonuses { get; set; }
        public ICollection<Absent> WorkChecks { get; set; }
        public ICollection<Vacation> Vacations { get; set; }
    }
}