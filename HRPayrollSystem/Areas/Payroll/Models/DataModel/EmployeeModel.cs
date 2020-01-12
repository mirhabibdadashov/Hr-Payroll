using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Admin.Models.DataModel
{
    public class EmployeeModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string BirthDate { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string VillageRegister { get; set; }
        [Required]
        public string IdentityNumber { get; set; }
        [Required]
        public string IdentityLastDay { get; set; }
        [Required]
        public Education Education { get; set; }
        [Required]
        public FamilyStatus FamilyStatus { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }
    }
}
