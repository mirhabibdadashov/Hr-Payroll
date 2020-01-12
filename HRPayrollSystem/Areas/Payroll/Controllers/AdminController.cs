using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using HRPayrollSystem.Areas.Payroll.Models.DataModel;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRPayrollSystem.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Route("/[area]/[controller]/[action]/{id?}")]
    public class AdminController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private PayroleDbContext _db;
        public AdminController(SignInManager<IdentityUser> SignInManager,
                                    PayroleDbContext Db,
                                        UserManager<IdentityUser> UserManager)
        {
            _signInManager = SignInManager;
            _userManager = UserManager;
            _db = Db;
        }
        [HttpGet]
        public async Task<IActionResult> Holding()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Holding");
                    HoldingModel model = new HoldingModel
                    {
                        Holdings = await _db.Holdings.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Holding(HoldingModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Holding");
                    if (ModelState.IsValid)
                    {
                        Holding holding = await _db.Holdings
                                                        .Where(h => h.Name == model.Name)
                                                            .FirstOrDefaultAsync();
                        if (holding == null)
                        {
                            Holding new_holding = new Holding
                            {
                                Name = model.Name
                            };
                            await _db.Holdings.AddAsync(new_holding);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            ModelState.AddModelError("", "This holding already exist");
                            HoldingModel hm = new HoldingModel
                            {
                                Holdings = await _db.Holdings.ToListAsync(),
                                Name = model.Name
                            };
                            return View(hm);
                        }
                    }
                    else
                    {
                        HoldingModel hm = new HoldingModel
                        {
                            Holdings = await _db.Holdings.ToListAsync(),
                            Name = model.Name
                        };
                        return View(hm);
                    }
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Company()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Company");
                    CompanyModel model = new CompanyModel
                    {
                        Holdings = await _db.Holdings.ToListAsync(),
                        Companies = await _db.Companies
                                                .Include(c => c.Holding)
                                                    .ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Company(CompanyModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Company");
                    if (ModelState.IsValid)
                    {
                        Company company = await _db.Companies
                                                        .Where(c => c.Name == model.Name && c.HoldingId == model.HoldingId)
                                                            .FirstOrDefaultAsync();
                        if (company == null)
                        {
                            Company new_company = new Company
                            {
                                Name = model.Name,
                                HoldingId = model.HoldingId
                            };
                            await _db.Companies.AddAsync(new_company);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            ModelState.AddModelError("", "This Company already exist");
                        }
                    }
                    CompanyModel cm = new CompanyModel
                    {
                        Holdings = await _db.Holdings.ToListAsync(),
                        Companies = await _db.Companies.ToListAsync(),
                        Name = model.Name,
                        HoldingId = model.HoldingId
                    };
                    return View(cm);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Branch()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Branch");
                    BranchModel model = new BranchModel
                    {
                        Branches = await _db.Branchs
                                                .Include(b => b.Company)
                                                    .ToListAsync(),
                        Companies = await _db.Companies.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Branch(BranchModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Branch");
                    if (ModelState.IsValid)
                    {
                        Branch branch = await _db.Branchs
                                                    .Where(b => b.Name == model.Name && b.CompanyId==model.CompanyId)
                                                        .FirstOrDefaultAsync();
                        if (branch == null)
                        {
                            Branch new_branch = new Branch
                            {
                                Name = model.Name,
                                Address = model.Address,
                                CompanyId = model.CompanyId
                            };
                            await _db.Branchs.AddAsync(new_branch);
                            await _db.SaveChangesAsync();
                            await Initializer.CreateBP(await _db.Branchs
                                                                    .Where(b => b.Name == branch.Name && b.Address == branch.Address)
                                                                        .FirstOrDefaultAsync(), null, _db);
                        }
                        else
                        {
                            ModelState.AddModelError("", "This Branch already exist");
                        }
                    }
                    BranchModel bm = new BranchModel
                    {
                        Branches = await _db.Branchs.ToListAsync(),
                        Companies = await _db.Companies.ToListAsync(),
                        Name = model.Name,
                        Address = model.Address,
                        CompanyId = model.CompanyId
                    };
                    return View(bm);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Department()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Department");
                    DepartmentModel model = new DepartmentModel
                    {
                        Departments = await _db.Departments.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Department(DepartmentModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Department");
                    if (ModelState.IsValid)
                    {
                        Department old_department = await _db.Departments
                                                                .Where(d => d.Name == model.Name)
                                                                    .FirstOrDefaultAsync();
                        if (old_department == null)
                        {
                            Department department = new Department
                            {
                                Name = model.Name
                            };
                            await _db.Departments.AddAsync(department);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            ModelState.AddModelError("", "This department already exist");
                        }
                    }
                    DepartmentModel dm = new DepartmentModel
                    {
                        Departments = await _db.Departments.ToListAsync(),
                        Name = model.Name
                    };
                    return View(dm);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Position()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Position");
                    PositionModel model = new PositionModel
                    {
                        Positions = await _db.Positions
                                                .Include(p => p.Salary)
                                                    .Include(p => p.Department)
                                                        .ToListAsync(),
                        Departments = await _db.Departments.ToListAsync(),
                        Salaries = await _db.Salaries.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Position(PositionModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Position");
                    if (ModelState.IsValid)
                    {
                        Position old_position = await _db.Positions
                                                            .Where(p => p.Name == model.Name && p.DepartmentId == model.DepartmentId)
                                                                .FirstOrDefaultAsync();
                        if (old_position == null)
                        {
                            Position position = new Position
                            {
                                Name = model.Name,
                                SalaryId = model.SalaryId,
                                DepartmentId = model.DepartmentId
                            };
                            await _db.Positions.AddAsync(position);
                            await _db.SaveChangesAsync();
                            await Initializer.CreateBP(null, await _db.Positions
                                                                        .Where(p => p.Name == position.Name)
                                                                            .FirstOrDefaultAsync(), _db);
                        }
                        else
                        {
                            ModelState.AddModelError("","This position already exist");
                        }
                    }
                    PositionModel pm = new PositionModel
                    {
                        Departments = await _db.Departments.ToListAsync(),
                        Name = model.Name,
                        SalaryId = model.SalaryId,
                        DepartmentId = model.DepartmentId
                    };
                    return View(pm);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Salary()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Salary");
                    SalaryModel model = new SalaryModel
                    {
                        Salaries = await _db.Salaries.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Salary(SalaryModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Salary");
                    if (ModelState.IsValid)
                    {
                        Salary old_salary = await _db.Salaries
                                                            .Where(s => s.Price == model.Price)
                                                                .FirstOrDefaultAsync();
                        if (old_salary == null)
                        {
                            Salary salary = new Salary
                            {
                                Price = model.Price
                            };
                            await _db.Salaries.AddAsync(salary);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            ModelState.AddModelError("", "This position already exist");
                        }
                    }
                    SalaryModel sm = new SalaryModel
                    {
                        Salaries = await _db.Salaries.ToListAsync(),
                        Price = model.Price
                    };
                    return View(sm);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Vacation(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Vacation");
                    VacationModel model = new VacationModel
                    {
                        Vacations = await _db.Vacations
                                                .Where(v => v.EmployeeId == id)
                                                    .Include(v => v.Employee)
                                                        .ToListAsync(),
                        EmployeeId = id
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Vacation(VacationModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Vacation");
                    VacationModel vm = new VacationModel();
                    if (!ModelState.IsValid)
                    {
                        ModelState.AddModelError("", "Something is missing");
                    }
                    else
                    {
                        Vacation vacation = new Vacation
                        {
                            StartTime = model.StartTime,
                            EndTime = model.EndTime,
                            EmployeeId = model.EmployeeId
                        };
                        await _db.Vacations.AddAsync(vacation);
                        await _db.SaveChangesAsync();
                        vm.StartTime = model.StartTime;
                        vm.EndTime = model.EndTime;
                        vm.EmployeeId = model.EmployeeId;
                    }
                    vm.Vacations = await _db.Vacations
                                            .Where(v => v.EmployeeId == model.EmployeeId)
                                                .Include(v => v.Employee)
                                                    .ToListAsync();
                    return View(vm);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> EditVacation(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Vacation/Edit");
                    Vacation vacation = await _db.Vacations
                                                    .Where(v => v.Id == id)
                                                        .FirstOrDefaultAsync();
                    VacationModel model = new VacationModel
                    {
                        Id = vacation.Id,
                        StartTime = vacation.StartTime,
                        EndTime = vacation.EndTime,
                        EmployeeId = vacation.EmployeeId
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> EditVacation(VacationModel v)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Vacation/Edit");
                    if (ModelState.IsValid)
                    {
                        Vacation vacation = await _db.Vacations
                                                        .Where(dbv => dbv.Id == v.Id)
                                                            .FirstOrDefaultAsync();
                        vacation.StartTime = v.StartTime;
                        vacation.EndTime = v.EndTime;
                        vacation.EmployeeId = v.EmployeeId;
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Vacation", "Admin", new { id = v.EmployeeId });
                    }
                    else
                    {
                        return View(v);
                    }
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Grade()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Grade");
                    GradeModel model = new GradeModel
                    {
                        Grades = await _db.Grades
                                            .Include(v => v.Branch)
                                                .ToListAsync(),
                        Branches = await _db.Branchs.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Grade(GradeModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Grade");
                    GradeModel gm = new GradeModel();
                    if (ModelState.IsValid)
                    {
                        Grade grade = new Grade
                        {
                            BranchId = model.BranchId,
                            Month = model.Month,
                            Year = model.Year,
                            From = model.From,
                            To = model.To,
                            Bonus = model.Bonus,
                            Cost = model.MinimumCost
                        };
                        await _db.Grades.AddAsync(grade);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        gm.BranchId = model.BranchId;
                        gm.Month = model.Month;
                        gm.From = model.From;
                        gm.To = model.To;
                        gm.Bonus = model.Bonus;
                        gm.MinimumCost = model.MinimumCost;
                        gm.Grades = await _db.Grades.ToListAsync();
                        ModelState.AddModelError("", "Something is missing");
                    }
                    gm.Branches = await _db.Branchs.ToListAsync();
                    gm.Grades = await _db.Grades.ToListAsync();
                    return View(gm);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> EditGrade(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Grade/Edit");
                    Grade grade = await _db.Grades
                                                .Where(g => g.Id == id)
                                                    .FirstOrDefaultAsync();
                    GradeModel model = new GradeModel
                    {
                        Id = grade.Id,
                        BranchId = grade.BranchId,
                        Month = grade.Month,
                        From = grade.From,
                        To = grade.To,
                        Bonus = grade.Bonus,
                        MinimumCost = grade.Cost,
                        Branches = await _db.Branchs.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> EditGrade(GradeModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Grade/Edit");
                    if (ModelState.IsValid)
                    {
                        Grade grade = await _db.Grades
                                                    .Where(g => g.Id == model.Id)
                                                        .FirstOrDefaultAsync();
                        grade.BranchId = model.BranchId;
                        grade.Month = model.Month;
                        grade.From = model.From;
                        grade.To = model.To;
                        grade.Bonus = model.Bonus;
                        grade.Cost = model.MinimumCost;
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Grade");
                    }
                    else
                    {
                        return View(model);
                    }
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Earning()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Earning");
                    EarningModel model = new EarningModel
                    {
                        Earnings = await _db.Earnings
                                                .Include(e => e.Branch)
                                                    .ToListAsync(),
                        Branches = await _db.Branchs.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> Earning(EarningModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Earning");
                    EarningModel em = new EarningModel();
                    if (ModelState.IsValid)
                    {
                        Earning earning = new Earning
                        {
                            BranchId = model.BranchId,
                            Month = model.Month,
                            Year = model.Year,
                            EarnCost = model.EarnCost
                        };
                        await _db.Earnings.AddAsync(earning);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        em.BranchId = model.BranchId;
                        em.Month = model.Month;
                        em.Year = model.Year;
                        em.EarnCost = model.EarnCost;
                        ModelState.AddModelError("", "Something is missing");
                    }
                    em.Branches = await _db.Branchs.ToListAsync();
                    em.Earnings = await _db.Earnings.Include(e => e.Branch).ToListAsync();
                    return View(em);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> EditEarning(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Earning/Edit");
                    Earning earning = await _db.Earnings
                                                    .Where(e => e.Id == id)
                                                        .FirstOrDefaultAsync();
                    EarningModel model = new EarningModel
                    {
                        Id = earning.Id,
                        BranchId = earning.BranchId,
                        Month = earning.Month,
                        Year = earning.Year,
                        EarnCost = earning.EarnCost,
                        Branches = await _db.Branchs.ToListAsync()
                    };
                    return View(model);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> EditEarning(EarningModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Earning/Edit");
                    if (ModelState.IsValid)
                    {
                        Earning earning = await _db.Earnings
                                                        .Where(e => e.Id == model.Id)
                                                            .FirstOrDefaultAsync();
                        earning.BranchId = model.BranchId;
                        earning.Month = model.Month;
                        earning.Year = model.Year;
                        earning.EarnCost = model.EarnCost;
                        await _db.SaveChangesAsync();
                        return RedirectToAction("Earning");
                    }
                    else
                    {
                        return View(model);
                    }
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> Role()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Role");
                    List<UserRoleModel> userRoleModels = new List<UserRoleModel>();
                    List<IdentityUserRole<string>> userRoles = await _db.UserRoles.ToListAsync();
                    foreach (IdentityUserRole<string> userRole in userRoles)
                    {
                        UserRoleModel userRoleModel = new UserRoleModel
                        {
                            User = await _db.Users
                                                .Where(u => u.Id == userRole.UserId)
                                                    .FirstOrDefaultAsync(),
                            Role = await _db.Roles
                                                .Where(r => r.Id == userRole.RoleId)
                                                    .FirstOrDefaultAsync()
                        };
                        userRoleModels.Add(userRoleModel);
                    }
                    return View(userRoleModels);
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Earning/Edit");
                    IdentityUser user = await _db.Users
                                                    .Where(u => u.Id == id)
                                                        .FirstOrDefaultAsync();
                    IdentityUserRole<string> userRole = await _db.UserRoles
                                                        .Where(ur => ur.UserId == user.Id)
                                                            .FirstOrDefaultAsync();
                    IdentityRole identityRole = await _db.Roles
                                                            .Where(ir => ir.Id == userRole.RoleId)
                                                                .FirstOrDefaultAsync();
                    UserRoleModel userRoleModel = new UserRoleModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        RoleName = identityRole.Name,
                        Roles = await _db.Roles.ToListAsync()
                    };
                    return View(userRoleModel);
                }
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(UserRoleModel userRoleModel)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Admin", ViewBag, _db, _signInManager, "Earning/Edit");
                    if (ModelState.IsValid)
                    {
                        //IdentityUser identityUser = await _db.Users
                        //                                        .Where(u => u.Id == userRoleModel.UserId)
                        //                                            .FirstOrDefaultAsync();
                        //identityUserRole.RoleId = identityRole.Id;
                        //_db.SaveChanges();
                        IdentityUser identityUser = await _userManager.FindByIdAsync(userRoleModel.UserId);
                        identityUser.UserName = userRoleModel.UserName;
                        identityUser.Email = userRoleModel.Email;
                        IdentityUserRole<string> identityUserRole = await _db.UserRoles
                                                         .Where(iur => iur.UserId == userRoleModel.UserId)
                                                             .FirstOrDefaultAsync();
                        IdentityRole identityRole = await _db.Roles
                                                                .Where(r => r.Id == identityUserRole.RoleId)
                                                                    .FirstOrDefaultAsync();
                        await _userManager.RemoveFromRoleAsync(identityUser, identityRole.Name);
                        await _userManager.AddToRoleAsync(identityUser, userRoleModel.RoleName);
                        return RedirectToAction("Role", "Admin");
                    }
                    else
                    {
                        UserRoleModel urm = new UserRoleModel
                        {
                            UserId = userRoleModel.UserId,
                            UserName = userRoleModel.UserName,
                            Email = userRoleModel.Email,
                            RoleName = (await _db.Roles
                                                    .Where(r => r.Name == userRoleModel.RoleName)
                                                        .FirstOrDefaultAsync()).Name,
                            Roles = await _db.Roles.ToListAsync()
                        };
                        return View(urm);
                    }
                }
            }
            return BadRequest();
        }
        public async Task<JsonResult> AddRole(int id)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "Something is wrong. Please try again!!!" });
            }
            Employee employee = await _db.Employees
                                            .Where(e => e.Id == id)
                                                .FirstOrDefaultAsync();
            if (employee == null)
            {
                return Json(new { status = 400, message = "This employee is not exist" });
            }
            else
            {
                return Json(new { status = 200, data = employee });
            }
        }
        public async Task<JsonResult> GetEmployee(int id)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "Something is wrong. Please try again!!!" });
            }
            Employee employee = await _db.Employees
                                            .Where(e => e.Id == id)
                                                .FirstOrDefaultAsync();
            if (employee == null)
            {
                return Json(new { status = 400, message = "This employee is not exist" });
            }
            else
            {
                return Json(new { status = 200, data = employee });
            }
        }
        public async Task<JsonResult> ChangeHolding(string Name, int id)
        {
            if (Name == null || Name == "")
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            Holding holding = await _db.Holdings
                                            .Where(h => h.Id == id)
                                                .FirstOrDefaultAsync();
            holding.Name = Name;
            await _db.SaveChangesAsync();
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangeCompany(string Name, int id)
        {
            if (Name == null || Name == "")
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Company company = await _db.Companies
                                                .Where(c => c.Id == id)
                                                    .FirstOrDefaultAsync();
                company.Name = Name;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangeBranchName(string Name, int id)
        {
            if (Name == null || Name == "")
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Branch branch = await _db.Branchs
                                            .Where(c => c.Id == id)
                                                .FirstOrDefaultAsync();
                branch.Name = Name;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangeBranchAddress(string Name, int id)
        {
            if (Name == null || Name == "")
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Branch branch = await _db.Branchs
                                            .Where(c => c.Id == id)
                                                .FirstOrDefaultAsync();
                branch.Address = Name;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangeDepartment(string Name, int id)
        {
            if (Name == null || Name == "")
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Department department = await _db.Departments
                                                     .Where(c => c.Id == id)
                                                         .FirstOrDefaultAsync();
                department.Name = Name;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangePositionName(string Name, int id)
        {
            if (Name == null || Name == "")
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Position position = await _db.Positions
                                                .Where(c => c.Id == id)
                                                    .FirstOrDefaultAsync();
                position.Name = Name;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangePositionSalary(int PositionId, int SalaryId)
        {
            if (PositionId == 0 || SalaryId == 0)
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Position position = await _db.Positions
                                                .Where(c => c.Id == PositionId)
                                                    .FirstOrDefaultAsync();
                position.SalaryId = SalaryId;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangeSalary(int id, int classId)
        {
            if (classId == 0 || id == 0)
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Position position = await _db.Positions
                                            .Where(p => p.Id == classId)
                                                .FirstOrDefaultAsync();
                position.SalaryId = id;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangeCompanyHolding(int id, int classId)
        {
            if (classId == 0 || id == 0)
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Company company = await _db.Companies
                                                .Where(c => c.Id == classId)
                                                    .FirstOrDefaultAsync();
                company.HoldingId = id;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangeBranchCompany(int id, int classId)
        {
            if (classId == 0 || id == 0)
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Branch branch = await _db.Branchs
                                                .Where(b => b.Id == classId)
                                                    .FirstOrDefaultAsync();
                branch.CompanyId = id;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        public async Task<JsonResult> ChangePositionDepartment(int id, int classId)
        {
            if (classId == 0 || id == 0)
            {
                return Json(new { status = 400, message = "This field can't be empty!!!" });
            }
            else
            {
                Position position = await _db.Positions
                                                .Where(p => p.Id == classId)
                                                    .FirstOrDefaultAsync();
                position.DepartmentId = id;
                await _db.SaveChangesAsync();
            }
            return Json(new { status = 200 });
        }
        [HttpGet]
        public async Task<IActionResult> DeleteHolding(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    Holding holding = await _db.Holdings
                                            .Where(h => h.Id == id)
                                                .FirstOrDefaultAsync();
                    _db.Holdings.Remove(holding);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Holding");
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    Company company = await _db.Companies
                                                    .Where(h => h.Id == id)
                                                        .FirstOrDefaultAsync();
                    _db.Companies.Remove(company);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Company");
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    Department department = await _db.Departments
                                                    .Where(h => h.Id == id)
                                                        .FirstOrDefaultAsync();
                    _db.Departments.Remove(department);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Department");
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> DeletePosition(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    Position position = await _db.Positions
                                                    .Where(h => h.Id == id)
                                                        .FirstOrDefaultAsync();
                    _db.Positions.Remove(position);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Position");
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteSalary(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    Salary salary = await _db.Salaries
                                                    .Where(h => h.Id == id)
                                                        .FirstOrDefaultAsync();
                    _db.Salaries.Remove(salary);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Salary");
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteVacation(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    Vacation vacation = await _db.Vacations
                                                    .Where(h => h.Id == id)
                                                        .FirstOrDefaultAsync();
                    _db.Vacations.Remove(vacation);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Vacation", new { id = vacation.EmployeeId });
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    Grade grade = await _db.Grades
                                                .Where(h => h.Id == id)
                                                    .FirstOrDefaultAsync();
                    _db.Grades.Remove(grade);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Grade");
                }
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    if (id == null)
                    {
                        return BadRequest();
                    }
                    IdentityUser identityUser = await _db.Users
                                                            .Where(u => u.Id == id)
                                                                .FirstOrDefaultAsync();
                    Employee employee = await _db.Employees
                                                    .Where(e => e.IdentityUserId == identityUser.Id)
                                                        .FirstOrDefaultAsync();
                    employee.IdentityUserId = null;
                    await _db.SaveChangesAsync();
                    _db.Users.Remove(identityUser);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Role", "Admin");
                }
            }
            return BadRequest();
        }
    }
}