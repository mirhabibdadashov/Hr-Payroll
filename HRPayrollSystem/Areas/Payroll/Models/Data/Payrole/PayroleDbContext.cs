using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models.DataModel;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;

namespace HRPayrollSystem.Areas.Admin.Models
{
    public class PayroleDbContext : IdentityDbContext<IdentityUser>
    {
        public PayroleDbContext(DbContextOptions opt) : base(opt) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Branch> Branchs { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<Absent> Absents { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<SubPage> SubPages { get; set; }
        public DbSet<OldWork> OldWorks { get; set; }
        public DbSet<Holding> Holdings { get; set; }
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<BranchPosition> BranchPositions { get; set; }
        public DbSet<Earning> Earnings { get; set; }
        public DbSet<Calculate> Calculates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BranchPosition>()
                                        .HasKey(bp => new { bp.Id });
            builder.Entity<BranchPosition>()
                                         .HasOne(bp => bp.Branch)
                                             .WithMany(b => b.BranchPositions)
                                                 .HasForeignKey(bp => bp.BranchId);
            builder.Entity<BranchPosition>()
                                         .HasOne(bp => bp.Position)
                                             .WithMany(p => p.BranchPositions)
                                                 .HasForeignKey(bp => bp.PositionId);
            base.OnModelCreating(builder);
        }
    }
}