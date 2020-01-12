using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Areas.Payroll.Models.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRPayrollSystem.Areas.Payroll.Controllers
{

    [Area("Payroll")]
    [Route("/[area]/[controller]/[action]/{id?}")]
    public class DepartmentController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private PayroleDbContext _db;
        public DepartmentController(SignInManager<IdentityUser> SignInManager,
                                                       PayroleDbContext Db)
        {
            _signInManager = SignInManager;
            _db = Db;
        }
        [HttpGet]
        public async Task<IActionResult> Bonus(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Bonus");
                    BonusModel bm = new BonusModel
                    {
                        Bonuses = await _db.Bonuses
                                                .Where(b => b.EmployeeId == id)
                                                    .ToListAsync(),
                        Employee = await _db.Employees
                                                .Where(e => e.Id == id)
                                                    .FirstOrDefaultAsync()
                    };
                    return View(bm);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> Bonus(BonusModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Bonus");
                    BonusModel bm = new BonusModel
                    {
                        Employee = await _db.Employees
                                                .Where(e => e.Id == model.EmployeeId)
                                                    .FirstOrDefaultAsync()
                    };
                    if (ModelState.IsValid)
                    {
                        if (bm.Employee.LastCalculate < model.GivenDate)
                        {
                            Bonus bonus = new Bonus
                            {
                                GivenDate = model.GivenDate,
                                Price = model.Price,
                                EmployeeId = model.EmployeeId
                            };
                            await _db.Bonuses.AddAsync(bonus);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            ModelState.AddModelError("", "You can't add bonus to this time");
                        }
                    }
                    else
                    {
                        bm.GivenDate = model.GivenDate;
                        bm.Price = model.Price;
                    }
                    bm.Bonuses = await _db.Bonuses
                                            .Where(b => b.EmployeeId == model.EmployeeId)
                                                .ToListAsync();
                    return View(bm);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> EditBonus(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Bonus/Edit");
                    if (id != 0)
                    {
                        Bonus bonus = await _db.Bonuses
                                                    .Where(b => b.Id == id)
                                                        .FirstOrDefaultAsync();
                        return View(bonus);
                    }
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> EditBonus(Bonus bonus)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Bonus/Edit");
                    if (bonus != null)
                    {
                        Bonus EditBonus = await _db.Bonuses
                                                    .Where(b => b.Id == bonus.Id)
                                                        .Include(b => b.Employee)
                                                            .FirstOrDefaultAsync();
                        if (EditBonus != null)
                        {
                            EditBonus.Price = bonus.Price;
                            if (bonus.Employee.LastCalculate < bonus.GivenDate)
                            {
                                EditBonus.GivenDate = bonus.GivenDate;
                            }
                            else
                            {
                                ModelState.AddModelError("", "You can't change this time");
                            }
                            await _db.SaveChangesAsync();
                        }
                        return RedirectToAction("Bonus", new { id = EditBonus.EmployeeId });
                    }
                    return View(bonus);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteBonus(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Bonus");
                    if (id != 0)
                    {
                        Bonus bonus = await _db.Bonuses
                                                    .Where(b => b.Id == id)
                                                         .Include(b => b.Employee)
                                                             .FirstOrDefaultAsync();
                        if (bonus.Employee.LastCalculate < bonus.GivenDate)
                        {
                            _db.Bonuses.Remove(bonus);
                            await _db.SaveChangesAsync();
                        }
                        return RedirectToAction("Bonus", new { id = bonus.EmployeeId });
                    }
                    else
                        return BadRequest();
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> Penalty(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Penalty");
                    PenaltyModel pm = new PenaltyModel
                    {
                        Fines = await _db.Fines
                                            .Where(b => b.EmployeeId == id)
                                                .ToListAsync(),
                        Employee = await _db.Employees
                                                .Where(e => e.Id == id)
                                                    .FirstOrDefaultAsync()
                    };
                    return View(pm);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> Penalty(PenaltyModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Penalty");
                    PenaltyModel pm = new PenaltyModel
                    {
                        Employee = await _db.Employees
                                                .Where(e => e.Id == model.EmployeeId)
                                                    .FirstOrDefaultAsync()
                    };
                    if (ModelState.IsValid)
                    {
                        if (pm.Employee.LastCalculate < model.GivenDate)
                        {
                            Fine fine = new Fine
                            {
                                GivenDate = model.GivenDate,
                                Price = model.Price,
                                EmployeeId = model.EmployeeId
                            };
                            await _db.Fines.AddAsync(fine);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            ModelState.AddModelError("", "You can't add penalty to this time");
                        }
                    }
                    else
                    {
                        pm.GivenDate = model.GivenDate;
                        pm.Price = model.Price;
                    }
                    pm.Fines = await _db.Fines
                                            .Where(b => b.EmployeeId == model.EmployeeId)
                                                .ToListAsync();
                    return View(pm);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> EditPenalty(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Penalty/Edit");
                    if (id != 0)
                    {
                        Fine fine = await _db.Fines
                                                .Where(b => b.Id == id)
                                                    .FirstOrDefaultAsync();
                        return View(fine);
                    }
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> EditPenalty(Fine fine)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    await Initializer.InitialLayout("Department", ViewBag, _db, _signInManager, "Penalty/Edit");
                    if (fine != null)
                    {
                        Fine EditBonus = await _db.Fines
                                                    .Where(f => f.Id == fine.Id)
                                                        .Include(f=>f.Employee)
                                                            .FirstOrDefaultAsync();
                        if (EditBonus != null)
                        {
                            EditBonus.Price = fine.Price;
                            if (fine.Employee.LastCalculate < fine.GivenDate)
                            {
                                EditBonus.GivenDate = fine.GivenDate;
                            }
                            else
                            {
                                ModelState.AddModelError("", "You can't change this time");
                            }
                            await _db.SaveChangesAsync();
                        }
                        return RedirectToAction("Penalty", new { id = EditBonus.EmployeeId });
                    }
                    return View(fine);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> DeletePenalty(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (_signInManager.Context.User.IsInRole("DepartmentHead"))
                {
                    if (id != 0)
                    {
                        Fine fine = await _db.Fines
                                                .Where(f => f.Id == id)
                                                         .Include(f => f.Employee)
                                                             .FirstOrDefaultAsync();
                        if (fine.Employee.LastCalculate < fine.GivenDate)
                        {
                            _db.Fines.Remove(fine);
                            await _db.SaveChangesAsync();
                        }
                        return RedirectToAction("Penalty", new { id = fine.EmployeeId });
                    }
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
    }
}