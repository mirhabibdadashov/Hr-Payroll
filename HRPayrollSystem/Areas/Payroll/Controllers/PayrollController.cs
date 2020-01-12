using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using HRPayrollSystem.Areas.Payroll.Models;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using HRPayrollSystem.Areas.Payroll.Models.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRPayrollSystem.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Route("/[area]/Calculate/[action]/{id?}")]
    public class PayrollController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private PayroleDbContext _db;
        public PayrollController(SignInManager<IdentityUser> SignInManager,
                                                       PayroleDbContext Db)
        {
            _signInManager = SignInManager;
            _db = Db;
        }
        [HttpGet]
        public async Task<IActionResult> Employees()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Payrole Specialist"))
                {
                    await Initializer.InitialLayout("Payrole Specialist", ViewBag, _db, _signInManager, "All");
                    ICollection<Work> works = await _db.Works
                                                            .Where(w => w.LeaveTime == null)
                                                                .Include(w => w.Employee)
                                                                    .Include(w => w.BranchPosition.Position)
                                                                        .ThenInclude(p => p.Salary)
                                                                            .ToListAsync();
                    return View(works);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> Calculate(string ids)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Payrole Specialist"))
                {
                    await Initializer.InitialLayout("Payrole Specialist", ViewBag, _db, _signInManager, "All");
                    List<PayrollModel> payrollModels = new List<PayrollModel>();
                    List<string> str_ids = ids.Split(new[] { ',' }).ToList();
                    foreach (string str_id in str_ids)
                    {
                        int id = Convert.ToInt32(str_id);
                        Employee employee = await _db.Employees.Where(e => e.Id == id).FirstOrDefaultAsync();
                        if (employee.LastCalculate <= DateTime.Now)
                        {
                            PayrollModel pm = new PayrollModel();
                            Work work = await _db.Works
                                                    .Where(w => w.EmployeeId == id && w.LeaveTime == null)
                                                        .Include(w => w.Employee)
                                                            .Include(w => w.BranchPosition.Position.Salary)
                                                                .FirstOrDefaultAsync();
                            decimal BasicSalary = 0;
                            decimal TotalAbsentPenalty = 0;
                            int TotalAbsentDay = 0;
                            decimal TotalSalary = 0;
                            if (work.Employee.LastCalculate < work.EnterTime)
                            {
                                Work OldWork = await _db.Works
                                                            .Where(w => w.EmployeeId == id && w.LeaveTime == work.EnterTime)
                                                                 .Include(w => w.Employee)
                                                                     .Include(w => w.BranchPosition.Position.Salary)
                                                                         .FirstOrDefaultAsync();
                                TotalSalary += OldWork.BranchPosition.Position.Salary.Price * (Convert.ToDateTime(OldWork.LeaveTime).DayOfYear - OldWork.Employee.LastCalculate.DayOfYear) / (decimal)30.5;
                                List<Absent> OldAbsents = await _db.Absents
                                                                    .Where(a => a.From >= work.Employee.LastCalculate && a.To <= OldWork.LeaveTime && a.EmployeeId == id)
                                                                        .ToListAsync();
                                int OldAbsentDay = 0;
                                int NewAbsentDay = 0;
                                foreach (Absent absent in OldAbsents)
                                {
                                    OldAbsentDay += absent.TotalDay;
                                    if (absent.TotalDay != 1)
                                    {
                                        TotalAbsentPenalty += OldWork.BranchPosition.Position.Salary.Price * (decimal)(2 * absent.TotalDay / 30.5);
                                    }
                                }
                                decimal OldSalary = OldWork.BranchPosition.Position.Salary.Price * (decimal)((Convert.ToDateTime(OldWork.LeaveTime).DayOfYear - work.Employee.LastCalculate.DayOfYear) / 30.5);
                                List<Absent> NewAbsents = await _db.Absents
                                                                    .Where(a => a.From >= OldWork.LeaveTime && a.To <= DateTime.Now && a.EmployeeId == id)
                                                                        .ToListAsync();
                                TotalSalary += (work.BranchPosition.Position.Salary.Price / (decimal)30.5) * (DateTime.Now.DayOfYear - work.Employee.LastCalculate.DayOfYear);
                                foreach (Absent absent in NewAbsents)
                                {
                                    NewAbsentDay += absent.TotalDay;
                                    if (absent.TotalDay != 1)
                                    {
                                        TotalAbsentPenalty += OldWork.BranchPosition.Position.Salary.Price * (decimal)(2 * absent.TotalDay / 30.5);
                                    }
                                }
                                decimal NewSalary = work.BranchPosition.Position.Salary.Price * (decimal)((DateTime.Now.DayOfYear - work.EnterTime.DayOfYear) / 30.5);
                                TotalAbsentDay = OldAbsentDay + NewAbsentDay;
                                BasicSalary = OldSalary + NewSalary;
                            }
                            else
                            {
                                List<Absent> absents = await _db.Absents
                                                                    .Where(a => a.From >= work.Employee.LastCalculate && a.To <= DateTime.Now && a.EmployeeId == id)
                                                                        .ToListAsync();
                                foreach (Absent absent in absents)
                                {
                                    TotalAbsentDay += absent.TotalDay;
                                    if (absent.TotalDay != 1)
                                    {
                                        TotalAbsentPenalty += work.BranchPosition.Position.Salary.Price * (decimal)(2 * absent.TotalDay / 30.5);
                                    }
                                }
                                BasicSalary = work.BranchPosition.Position.Salary.Price * (decimal)((DateTime.Now.DayOfYear - work.Employee.LastCalculate.DayOfYear - TotalAbsentDay) / 30.5);
                            }
                            List<Bonus> bonuses = await _db.Bonuses
                                                                .Where(b => b.GivenDate >= work.Employee.LastCalculate && b.GivenDate <= DateTime.Now && b.EmployeeId == id)
                                                                    .ToListAsync();
                            decimal TotalBonuses = 0;
                            decimal TotalFines = 0;
                            if (bonuses.Count > 0)
                            {
                                foreach (Bonus bonus in bonuses)
                                {
                                    TotalBonuses += bonus.Price;
                                }
                                List<Fine> fines = await _db.Fines
                                                                    .Where(b => b.GivenDate >= work.Employee.LastCalculate && b.GivenDate <= DateTime.Now && b.EmployeeId == id)
                                                                        .ToListAsync();
                                if (fines.Count > 0)
                                {
                                    foreach (Fine fine in fines)
                                    {
                                        TotalFines += fine.Price;
                                    }
                                }
                            }
                            List<Grade> grades = await _db.Grades
                                                            .Where(g => g.BranchId == work.BranchPosition.BranchId && g.Month == DateTime.Now.Month && g.Year == DateTime.Now.Year)
                                                                .ToListAsync();
                            decimal TotalGrade = 0;
                            if (grades.Count > 0)
                            {
                                foreach (Grade grade in grades)
                                {
                                    Earning earning = await _db.Earnings
                                                                    .Where(e => e.BranchId == work.BranchPosition.BranchId && e.Month == grade.Month && e.Year == grade.Year)
                                                                        .FirstOrDefaultAsync();
                                    if (earning != null && grade.From <= earning.EarnCost && grade.To >= earning.EarnCost)
                                    {
                                        if (grade.Cost == Cost.Manat)
                                        {
                                            TotalGrade += grade.Bonus;
                                        }
                                        else
                                        {
                                            TotalGrade += work.BranchPosition.Position.Salary.Price * (grade.Bonus / 100);
                                        }
                                    }
                                }
                            }
                            List<Vacation> vacations = await _db.Vacations
                                                                    .Where(v => v.StartTime >= work.Employee.LastCalculate && v.EndTime <= DateTime.Now && v.EmployeeId == id)
                                                                        .ToListAsync();
                            decimal TotalVacationPrize = 0;
                            int TotalVacationDays = 0;
                            if (vacations.Count > 0)
                            {
                                foreach (Vacation vacation in vacations)
                                {
                                    TotalVacationDays += vacation.EndTime.DayOfYear - vacation.StartTime.DayOfYear;
                                    TotalVacationPrize += (work.BranchPosition.Position.Salary.Price * (decimal)((vacation.EndTime.DayOfYear - vacation.StartTime.DayOfYear) / 30.5)) / 2;
                                }
                            }
                            employee.LastCalculate = DateTime.Now;
                            await _db.SaveChangesAsync();
                            Calculate calculate = new Calculate
                            {
                                EmployeeId=employee.Id,
                                TotalAbsentDays = TotalAbsentDay,
                                TotalBonuses = TotalBonuses,
                                TotalPenalties = TotalFines,
                                TotalSalary = BasicSalary + TotalBonuses + TotalGrade + TotalVacationPrize - TotalFines - TotalAbsentPenalty,
                                TotalVacationDays = TotalVacationDays,
                                CalculateDate = DateTime.Now
                            };
                            await _db.Calculates.AddAsync(calculate);
                            await _db.SaveChangesAsync();
                            pm.Salary = work.BranchPosition.Position.Salary.Price;
                            pm.TotalBonus = TotalBonuses;
                            pm.TotalPenalties = TotalFines;
                            pm.TotalGrade = TotalGrade;
                            pm.TotalVacation = TotalVacationPrize;
                            pm.TotalAbsentPenalty = TotalAbsentPenalty;
                            pm.TotalAbsentDay = TotalAbsentDay;
                            pm.TotalSalary = BasicSalary + TotalBonuses + TotalGrade + TotalVacationPrize - TotalFines - TotalAbsentPenalty;
                            pm.Work = work;
                            payrollModels.Add(pm);
                        }
                    }
                    return View(payrollModels);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> AllCalculates(int? Month)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Payrole Specialist"))
                {
                    await Initializer.InitialLayout("Payrole Specialist", ViewBag, _db, _signInManager, "All");
                    if (Month == null)
                    {
                        Month = DateTime.Now.Month;
                    }
                    List<Calculate> calculates = await _db.Calculates
                                                            .Where(c=>c.CalculateDate.Month==Month)
                                                                .Include(c=>c.Employee)
                                                                    .ToListAsync();
                    CalculateModel calculateModel = new CalculateModel
                    {
                        Calculates=calculates,
                        Month=Convert.ToInt32(Month)
                    };
                    return View(calculateModel);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
    }
}