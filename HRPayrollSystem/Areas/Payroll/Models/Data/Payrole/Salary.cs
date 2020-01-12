using System.Collections.Generic;

namespace HRPayrollSystem.Areas.Admin.Models
{
    public class Salary
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public ICollection<Position> Positions { get; set; }
    }
}