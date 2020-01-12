using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Models
{
    public class Seed
    {
        public static async Task InvokeAsync(IServiceScope scope, PayroleDbContext db)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            if (!db.Users.Any())
            {
                IdentityUser Admin = new IdentityUser
                {
                    UserName = "Admin",
                    Email = "Admin@gmail.com"
                };
                IdentityResult AdminCreateResult = await userManager.CreateAsync(Admin, configuration["Passwords:Admin"]);
                if (!db.Roles.Any())
                {
                    string[] roles = { "Admin", "Hr", "Payrole Specialist", "DepartmentHead" };
                    foreach (string role in roles)
                    {
                        await roleManager.CreateAsync(new IdentityRole
                        {
                            Name = role
                        });
                    }
                }
                if (AdminCreateResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(Admin, "Admin");
                };

                IdentityUser Hr = new IdentityUser
                {
                    UserName = "Hr",
                    Email = "Hr@gmail.com"
                };
                IdentityResult HrCreateResult = await userManager.CreateAsync(Hr, configuration["Passwords:Hr"]);
                if (HrCreateResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(Hr, "Hr");
                };

                IdentityUser DepartmentHead = new IdentityUser
                {
                    UserName = "DepartmentHead",
                    Email = "DH@gmail.com"
                };
                IdentityResult DepartmentHeadCreateResult = await userManager.CreateAsync(DepartmentHead, configuration["Passwords:DH"]);
                if (DepartmentHeadCreateResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(DepartmentHead, "DepartmentHead");
                };

                IdentityUser PayroleSpecialist = new IdentityUser
                {
                    UserName = "PayroleSpecialist",
                    Email = "PS@gmail.com"
                };
                IdentityResult PayroleSpecialistCreateResult = await userManager.CreateAsync(PayroleSpecialist, configuration["Passwords:PS"]);
                if (PayroleSpecialistCreateResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(PayroleSpecialist, "Payrole Specialist");
                };
            }
            if (!db.Holdings.Any())
            {
                Holding holding = new Holding
                {
                    Name = "Munir Holding"
                };
                await db.Holdings.AddAsync(holding);
                await db.SaveChangesAsync();
            }
            if (db.Holdings.Any() && !db.Companies.Any())
            {
                List<string> companies = new List<string>
                        {
                            "New Yorker","Emporium","Celio","Mango"
                        };
                foreach (string company in companies)
                {
                    Company c = new Company
                    {
                        Name = company,
                        HoldingId = db.Holdings.Where(h => h.Name == "Munir Holding").FirstOrDefault().Id
                    };
                    await db.Companies.AddAsync(c);
                }
                await db.SaveChangesAsync();
            }
            if (db.Companies.Any()&& !db.Branchs.Any())
            {
                Branch branch = new Branch
                {
                    Name = "28 May",
                    Address = "28 Mall 2ci mərtəbə",
                    CompanyId = db.Companies
                                        .Where(c => c.Name == "New Yorker")
                                            .FirstOrDefault().Id
                };
                await db.Branchs.AddAsync(branch);
                await db.SaveChangesAsync();
            }
            if (!db.Departments.Any())
            {
                List<string> Departments = new List<string>
                        {
                            "Sales","IT","Economic Increase","Marketing"
                        };
                foreach (string Department in Departments)
                {
                    Department d = new Department
                    {
                        Name = Department
                    };
                    await db.Departments.AddAsync(d);
                }
                await db.SaveChangesAsync();
            }
            if (!db.Salaries.Any())
            {
                List<Salary> salaries = new List<Salary>
                {
                    new Salary
                    {
                        Price =500
                    },
                    new Salary
                    {
                        Price =750
                    },
                    new Salary
                    {
                        Price =1000
                    },
                    new Salary
                    {
                        Price =1500
                    }
                };
                foreach (Salary salary in salaries)
                {
                    await db.Salaries.AddAsync(salary);
                }
                await db.SaveChangesAsync();
            }
            if (db.Departments.Any() && db.Salaries.Any() && !db.Positions.Any())
            {
                List<string> SalePositions = new List<string>
                        {
                            "Sales Representative","Sales Representative's assistant","Main Salesman"
                        };
                foreach (string Position in SalePositions)
                {
                    Position p = new Position
                    {
                        Name = Position,
                        DepartmentId = db.Departments.Where(d => d.Name == "Sales").FirstOrDefault().Id,
                        SalaryId = db.Salaries
                                        .Where(s=>s.Price==500)
                                            .FirstOrDefault().Id
                    };
                    await db.Positions.AddAsync(p);
                }
                List<string> EIPositions = new List<string>
                {
                    "Branch Manager","Brend Manager"
                };
                foreach (string Position in EIPositions)
                {
                    Position p = new Position
                    {
                        Name = Position,
                        DepartmentId = db.Departments.Where(d => d.Name == "Economic Increase").FirstOrDefault().Id,
                        SalaryId = db.Salaries
                                        .Where(s => s.Price == 750)
                                            .FirstOrDefault().Id
                    };
                    await db.Positions.AddAsync(p);
                }
                List<string> ITPositions = new List<string>
                {
                    "Programist","Main Programist","Lead Programist","IT Director","System Administrator"
                };
                foreach (string Position in ITPositions)
                {
                    Position p = new Position
                    {
                        Name = Position,
                        DepartmentId = db.Departments.Where(d => d.Name == "IT").FirstOrDefault().Id,
                        SalaryId = db.Salaries
                                        .Where(s => s.Price == 1000)
                                            .FirstOrDefault().Id
                    };
                    await db.Positions.AddAsync(p);
                }
                List<string> MarketingPositions = new List<string>
                {
                    "Hr","Payrole Specialist","DepartmentHead"
                };
                foreach (string Position in MarketingPositions)
                {
                    Position p = new Position
                    {
                        Name = Position,
                        DepartmentId = db.Departments.Where(d => d.Name == "Marketing").FirstOrDefault().Id,
                        SalaryId = db.Salaries
                                        .Where(s => s.Price == 1500)
                                            .FirstOrDefault().Id
                    };
                    await db.Positions.AddAsync(p);
                }
                await db.SaveChangesAsync();
            }
            if(db.Branchs.Any() && db.Positions.Any() && !db.BranchPositions.Any())
            {
                List<Branch> Branches = await db.Branchs.ToListAsync();
                List<Position> Positions = await db.Positions.ToListAsync();
                foreach (Branch branch in Branches)
                {
                    foreach(Position position in Positions)
                    {
                        await db.BranchPositions.AddAsync(new BranchPosition { BranchId = branch.Id, PositionId = position.Id });
                        await db.SaveChangesAsync();
                    }
                }
            }
            if (!db.Pages.Any())
            {
                List<Page> pages = new List<Page>
                {
                    new Page{
                    Name = "Employees",
                    Link = "Hr",
                    Icon = "icon-users"
                    },
                    new Page{
                    Name = "Work Places",
                    Link = "Work",
                    Icon = "fas fa-briefcase"
                    },
                    new Page{
                    Name = "Admin",
                    Link = "Admin",
                    Icon = "fas fa-briefcase"
                    },
                    new Page{
                    Name = "Department",
                    Link = "Department",
                    Icon = "fas fa-briefcase"
                    },
                    new Page{
                    Name = "Payrole",
                    Link = "Payrole",
                    Icon = "fas fa-briefcase"
                    }
                };
                foreach (Page page in pages)
                {
                    await db.Pages.AddAsync(page);
                }
                await db.SaveChangesAsync();
            }
            if (db.Pages.Any() && !db.SubPages.Any())
            {
                Page EmployeePage = await db.Pages.Where(p => p.Name == "Employees").FirstOrDefaultAsync();
                Page WorkPage = await db.Pages.Where(p => p.Name == "Work Places").FirstOrDefaultAsync();
                Page AdminPage = await db.Pages.Where(p => p.Name == "Admin").FirstOrDefaultAsync();
                Page DepartmentPage = await db.Pages.Where(p => p.Name == "Department").FirstOrDefaultAsync();
                Page PayrolePage = await db.Pages.Where(p => p.Name == "Payrole").FirstOrDefaultAsync();
                List<SubPage> subPages = new List<SubPage>
                {
                    new SubPage{
                    Name = "All Employees",
                    Link = "Index",
                    PageId=EmployeePage.Id
                    },
                    new SubPage{
                    Name = "Add Employee",
                    Link = "Add",
                    PageId=EmployeePage.Id
                    },
                    new SubPage{
                    Name = "Edit Employee",
                    Link = "Edit",
                    PageId=EmployeePage.Id
                    },
                    new SubPage{
                    Name = "Employee Attendance",
                    Link = "Workcheck",
                    PageId=EmployeePage.Id
                    },
                    new SubPage{
                    Name = "Employee work places",
                    Link = "Index",
                    PageId=WorkPage.Id
                    },
                    new SubPage{
                    Name = "Add Old work places",
                    Link = "AddOld",
                    PageId=WorkPage.Id
                    },
                    new SubPage{
                    Name = "Add New work places",
                    Link = "AddNew",
                    PageId=WorkPage.Id
                    },
                    new SubPage{
                    Name = "Holding",
                    Link = "Holding",
                    PageId=AdminPage.Id
                    },
                    new SubPage{
                    Name = "Company",
                    Link = "Company",
                    PageId=AdminPage.Id
                    },
                    new SubPage{
                    Name = "Branch",
                    Link = "Branch",
                    PageId=AdminPage.Id
                    },
                    new SubPage{
                    Name = "Department",
                    Link = "Department",
                    PageId=AdminPage.Id
                    },
                    new SubPage{
                    Name = "Position",
                    Link = "Position",
                    PageId=AdminPage.Id
                    },
                    new SubPage{
                    Name = "Vacation",
                    Link = "Vacation",
                    PageId=AdminPage.Id
                    },
                    new SubPage{
                    Name = "Grade",
                    Link = "Grade",
                    PageId=AdminPage.Id
                    },
                    new SubPage{
                    Name = "Earning",
                    Link = "Earning",
                    PageId=AdminPage.Id
                    },
                    new SubPage{
                    Name = "Bonus",
                    Link = "Bonus",
                    PageId=DepartmentPage.Id
                    },
                    new SubPage{
                    Name = "Penalty",
                    Link = "Penalty",
                    PageId=DepartmentPage.Id
                    }
                };
                foreach (SubPage subPage in subPages)
                {
                    await db.SubPages.AddAsync(subPage);
                }
                await db.SaveChangesAsync();
            }
        }
    }
}