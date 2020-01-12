using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using HRPayrollSystem.Areas.Admin.Models.DataModel;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRPayrollSystem.Areas.Admin.Controllers
{
    [Area("Payroll")]
    [Route("/[area]/Work/[action]/{id?}")]
    public class WorkPlacesController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private PayroleDbContext _db;
        public WorkPlacesController(SignInManager<IdentityUser> SignInManager,
                                                       PayroleDbContext Db)
        {
            _signInManager = SignInManager;
            _db = Db;
        }
        [HttpGet]
        public async Task<IActionResult> Works(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "Employee work places");
                    Employee employee = await _db.Employees
                                                    .Where(e => e.Id == id)
                                                        .FirstOrDefaultAsync();
                    List<OldWork> oldWorks = await _db.OldWorks
                                                        .OrderBy(ow => ow.EnterTime)
                                                            .Where(ow => ow.EmployeeId == id)
                                                                .ToListAsync();
                    List<Work> works = await _db.Works
                                                    .Where(w => w.EmployeeId == id)
                                                        .OrderBy(w => w.EnterTime)
                                                            .Include(w => w.BranchPosition)
                                                                .ThenInclude(bp => bp.Branch)
                                                                    .ThenInclude(b => b.Company)
                                                                        .ThenInclude(c => c.Holding)
                                                                            .Include(w => w.BranchPosition)
                                                                                .ThenInclude(bp => bp.Position)
                                                                                    .ThenInclude(p => p.Department)
                                                                                        .ToListAsync();
                    WorksModel wm = new WorksModel
                    {
                        OldWorks = oldWorks,
                        Works = works,
                        Employee = employee
                    };
                    return View(wm);
                }
            }
            return RedirectToAction("Forbidden","Error");
        }
        [HttpGet]
        public async Task<IActionResult> AddOld(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "Add Old work places");
                    Employee employee = await _db.Employees
                                                    .Where(e => e.Id == id)
                                                        .FirstOrDefaultAsync();
                    if (employee == null)
                    {
                        return BadRequest();
                    }
                    OldWorkModel owm = new OldWorkModel
                    {
                        Employee = employee,
                        EmployeeId = id
                    };
                    return View(owm);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> AddOld(OldWorkModel owm)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "Add Old work places");
                    if (ModelState.IsValid)
                    {
                        OldWork ow = new OldWork
                        {
                            HoldingName = owm.HoldingName,
                            CompanyName = owm.CompanyName,
                            BranchName = owm.BranchName,
                            DepartmentName = owm.DepartmentName,
                            PositionName = owm.PositionName,
                            EnterTime = owm.EnterTime,
                            LeaveTime = owm.LeaveTime,
                            Reason = owm.Reason,
                            EmployeeId = owm.EmployeeId
                        };
                        await _db.OldWorks.AddAsync(ow);
                        await _db.SaveChangesAsync();
                        return Redirect("/Payroll/Work/Works/" + owm.EmployeeId);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Something is wrong. Please try again");
                        return View(owm);
                    }
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> EditOld(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "Old/Edit");
                    OldWorkModel oldWork = await _db.OldWorks
                                                    .Where(ow => ow.Id == id)
                                                        .Include(ow => ow.Employee)
                                                            .Select(ow => new OldWorkModel
                                                            {
                                                                Id = ow.Id,
                                                                HoldingName = ow.HoldingName,
                                                                CompanyName = ow.CompanyName,
                                                                BranchName = ow.BranchName,
                                                                DepartmentName = ow.DepartmentName,
                                                                PositionName = ow.PositionName,
                                                                EnterTime = ow.EnterTime,
                                                                LeaveTime = ow.LeaveTime,
                                                                Reason = ow.Reason,
                                                                Employee = ow.Employee,
                                                                EmployeeId = ow.EmployeeId
                                                            })
                                                                .FirstOrDefaultAsync();
                    return View(oldWork);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> EditOld(OldWorkModel oldWorkModel)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "Old/Edit");
                    if (ModelState.IsValid)
                    {
                        OldWork ow = await _db.OldWorks
                                            .Where(dbow => dbow.Id == oldWorkModel.Id)
                                                .Include(dbow => dbow.Employee)
                                                    .FirstOrDefaultAsync();
                        if (ow == null)
                        {
                            ModelState.AddModelError("", "This work places couldn't find");
                            return View(oldWorkModel);

                        }
                        ow.HoldingName = oldWorkModel.HoldingName;
                        ow.BranchName = oldWorkModel.BranchName;
                        ow.CompanyName = oldWorkModel.CompanyName;
                        ow.DepartmentName = oldWorkModel.DepartmentName;
                        ow.PositionName = oldWorkModel.PositionName;
                        ow.EnterTime = oldWorkModel.EnterTime;
                        ow.LeaveTime = oldWorkModel.LeaveTime;
                        ow.Reason = oldWorkModel.Reason;
                        await _db.SaveChangesAsync();
                        return Redirect("/Payroll/Work/Works/" + ow.EmployeeId);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Something is wrong. Please try again");
                        return View(oldWorkModel);
                    }
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteOld(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    OldWork oldWork = await _db.OldWorks
                                        .Where(ow => ow.Id == id)
                                            .Include(ow => ow.Employee)
                                                .FirstOrDefaultAsync();
                    if (oldWork == null)
                    {
                        ModelState.AddModelError("", "This work places couldn't find");
                        return BadRequest();
                    }
                    _db.OldWorks.Remove(oldWork);
                    await _db.SaveChangesAsync();
                    return Redirect("/Payroll/Work/Works/" + oldWork.EmployeeId);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> AddNew(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "Add New work places");
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    Employee employee = await _db.Employees
                                                .Where(e => e.Id == id)
                                                    .FirstOrDefaultAsync();
                    List<Holding> holdings = await _db.Holdings.ToListAsync();
                    List<Department> departments = await _db.Departments.ToListAsync();
                    WorkModel model = new WorkModel
                    {
                        EmployeeId = employee.Id,
                        Employee = employee,
                        Holdings = holdings,
                        Departments = departments
                    };
                    return View(model);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        public async Task<JsonResult> GetCompanies(int id)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this holding" });
            }
            List<Company> companies = await _db.Companies
                                                    .Where(c => c.HoldingId == id)
                                                        .ToListAsync();
            if (companies.Count == 0)
            {
                return Json(new { status = 400, message = "We couldn't find companies" });
            }
            else
            {
                return Json(new { status = 200, data = companies });
            }
        }
        public async Task<JsonResult> GetBranches(int id)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this company" });
            }
            List<Branch> branches = await _db.Branchs
                                                .Where(b => b.CompanyId == id)
                                                    .ToListAsync();
            if (branches.Count == 0)
            {
                return Json(new { status = 400, message = "We couldn't find branches" });
            }
            else
            {
                return Json(new { status = 200, data = branches });
            }
        }
        public async Task<JsonResult> GetPositions(int id)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this department" });
            }
            List<Position> positions = await _db.Positions
                                                    .Where(p => p.DepartmentId == id)
                                                        .ToListAsync();
            if (positions.Count == 0)
            {
                return Json(new { status = 400, message = "We couldn't find positions" });
            }
            else
            {
                return Json(new { status = 200, data = positions });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNew(WorkModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "Add New work places");
                    Employee employee = await _db.Employees
                                                    .Where(e => e.Id == model.EmployeeId)
                                                        .FirstOrDefaultAsync();
                    List<Holding> holdings = await _db.Holdings.ToListAsync();
                    List<Department> departments = await _db.Departments.ToListAsync();
                    WorkModel workModel = new WorkModel
                    {
                        EmployeeId = employee.Id,
                        Employee = employee,
                        Holdings = holdings,
                        Departments = departments
                    };
                    if (ModelState.IsValid)
                    {

                        BranchPosition branchPosition = await _db.BranchPositions
                                                                    .Where(bp => bp.PositionId == model
                                                                        .PositionId && bp.BranchId == model.BranchId)
                                                                            .FirstOrDefaultAsync();
                        Work work = new Work
                        {
                            BranchPositionId = branchPosition.Id,
                            EnterTime = model.EnterTime,
                            EmployeeId = model.EmployeeId
                        };
                        await _db.Works.AddAsync(work);
                        await _db.SaveChangesAsync();
                        return Redirect("/Payroll/Work/Works/" + workModel.EmployeeId);
                    }
                    else
                    {
                        workModel.BranchId = model.BranchId;
                        workModel.PositionId = model.PositionId;
                        workModel.EnterTime = model.EnterTime;
                        return View(workModel);
                    }

                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> EditNew(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    if (id == 0)
                    {
                        return BadRequest();
                    }
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "New/Edit");
                    WorkModel Work = await _db.Works
                                                .Where(w => w.Id == id)
                                                    .Include(w => w.Employee)
                                                        .Include(w => w.BranchPosition)
                                                            .ThenInclude(w => w.Branch)
                                                                .Include(w => w.BranchPosition)
                                                                    .ThenInclude(w => w.Position)
                                                                        .Select(w => new WorkModel
                                                                        {
                                                                            Id = w.Id,
                                                                            BranchPositionId = w.BranchPositionId,
                                                                            EnterTime = w.EnterTime,
                                                                            LeaveTime = Convert.ToDateTime(w.LeaveTime),
                                                                            Reason = w.Reason,
                                                                            Employee = w.Employee,
                                                                            EmployeeId = w.EmployeeId
                                                                        })
                                                                            .FirstOrDefaultAsync();
                    BranchPosition branchPosition = await _db.BranchPositions
                                                                .Where(bp => bp.Id == Work.BranchPositionId)
                                                                    .Include(bp => bp.Position)
                                                                        .ThenInclude(p=>p.Department)
                                                                            .FirstOrDefaultAsync();
                    Work.BranchPosition = branchPosition;
                    Position position = branchPosition.Position;
                    Work.BranchPositions = await _db.BranchPositions
                                                        .Where(bp => bp.PositionId == position.Id)
                                                            .Include(bp => bp.Position)
                                                                .Include(bp=>bp.Branch)
                                                                    .ThenInclude(b=>b.Company)
                                                                        .ThenInclude(c=>c.Holding)
                                                                            .ToListAsync();
                    Work.Departments = await _db.Departments.ToListAsync();
                    Work.Position = position;
                    Work.Holdings = await _db.Holdings.ToListAsync();
                    return View(Work);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> EditNew(WorkModel WorkModel)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Work Places", ViewBag, _db, _signInManager, "New/Edit");
                    if (ModelState.IsValid)
                    {
                        Work work = await _db.Works
                                            .Where(w => w.Id == WorkModel.Id)
                                                .Include(w => w.Employee)
                                                    .Include(w=>w.BranchPosition)
                                                        .ThenInclude(bp=>bp.Position)
                                                            .Include(w=>w.BranchPosition.Branch)
                                                                .ThenInclude(b=>b.Company)
                                                                    .ThenInclude(c=>c.Holding)
                                                                        .FirstOrDefaultAsync();
                        if (work == null)
                        {
                            ModelState.AddModelError("", "This work places couldn't find");
                            return View(WorkModel);
                        }
                        if (work.BranchPosition.PositionId != WorkModel.PositionId)
                        {
                            work.LeaveTime = WorkModel.LeaveTime;
                            work.Reason = WorkModel.Reason;
                            Work new_work = new Work
                            {
                                BranchPositionId = WorkModel.BranchPositionId,
                                EnterTime = WorkModel.LeaveTime,
                                EmployeeId = work.EmployeeId
                            };
                            await _db.Works.AddAsync(new_work);
                        }
                        else
                        {
                            work.EnterTime = WorkModel.EnterTime;
                        }
                        await _db.SaveChangesAsync();
                        return Redirect("/Payroll/Work/Works/" + work.EmployeeId);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Something is wrong. Please try again");
                        return View(WorkModel);
                    }
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        public async Task<JsonResult> GetCompaniesAndSelected(int id, int positionId,int? branchId)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this holding" });
            }
            else if (positionId == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this holding" });
            }
            List<Company> companies = await _db.Companies
                                                    .Where(c => c.HoldingId == id)
                                                        .ToListAsync();
            if (companies.Count == 0)
            {
                return Json(new { status = 400, message = "We couldn't find companies" });
            }
            else
            {
                return Json(new { status = 200, data = companies, id = positionId });
            }
        }
        public async Task<JsonResult> GetBranchesAndSelected(int id, int positionId, int? branchId)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this company" });
            }
            else if (positionId == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this company" });
            }
            List<Branch> branches = await _db.Branchs
                                                .Where(b => b.CompanyId == id)
                                                    .ToListAsync();
            if (branches.Count == 0)
            {
                return Json(new { status = 400, message = "We couldn't find branches" });
            }
            else
            {
                return Json(new { status = 200, data = branches, id = positionId });
            }
        }
        public async Task<JsonResult> GetPositionsAndSelected(int id, int positionId, int? branchId)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this department" });
            }
            if (positionId == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this department" });
            }
            if (branchId == null || branchId == 0)
            {
                return Json(new { status = 400, message = "We couldn't find this brenches" });
            }
            List<Position> positions = await _db.Positions
                                                    .Where(p => p.Id == id)
                                                        .ToListAsync();
            List<BranchPosition> branchPositions = new List<BranchPosition>();
            foreach (Position position in positions)
            {
                List<BranchPosition> branches = await _db.BranchPositions
                                                            .Where(bp => bp.PositionId == position.Id && bp.BranchId == branchId)
                                                                .ToListAsync();
                foreach (BranchPosition branchPosition in branches)
                {
                    branchPositions.Add(branchPosition);
                }
            }
            if (branchPositions.Count == 0)
            {
                return Json(new { status = 400, message = "We couldn't find positions" });
            }
            else
            {
                return Json(new { status = 200, data = branchPositions, id = positionId });
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteNew(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    if (id!=0)
                    {
                        Work work = await _db.Works
                                            .Where(w => w.Id == id)
                                                .FirstOrDefaultAsync();
                        if (work != null)
                        {
                            _db.Works.Remove(work);
                            await _db.SaveChangesAsync();
                            return Redirect("/Payroll/Work/Works/" + work.EmployeeId);
                        }
                    }
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
    }
}