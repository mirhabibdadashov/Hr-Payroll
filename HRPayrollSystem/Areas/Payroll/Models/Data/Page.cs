using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Admin.Models
{
    public class Page
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Link { get; set; }
        [Required]
        public string Icon { get; set; }
        public ICollection<SubPage> SubPages { get; set; }
    }
}
