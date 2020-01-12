using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using HRPayrollSystem.Areas.Admin.Models.DataModel;
using HRPayrollSystem.Areas.Payroll.Models.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRPayrollSystem.Areas.Admin.Controllers
{
    [Area("Payroll")]
    [Route("/[area]/hr/[action]/{id?}")]
    public class EmployeeController : Controller
    {
        private SignInManager<IdentityUser> _signInManager;
        private PayroleDbContext _db;
        private IHostingEnvironment _hosting;
        private UserManager<IdentityUser> _userManager;
        public EmployeeController(SignInManager<IdentityUser> SignInManager,
                                                       PayroleDbContext Db,
                                                           IHostingEnvironment hosting,
                                                                UserManager<IdentityUser> UserManager)
        {
            _signInManager = SignInManager;
            _db = Db;
            _hosting = hosting;
            _userManager = UserManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int id=1)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Payrole Specialist"))
                {
                    return Redirect("/Payroll/Calculate/Employees");
                }
                else if (_signInManager.Context.User.IsInRole("Department"))
                {
                    await Initializer.InitialLayout("Employees", ViewBag, _db, _signInManager, "All");
                    List<Work> works = await _db.Works
                                                    .Include(w => w.BranchPosition)
                                                        .ThenInclude(bp => bp.Position)
                                                            .ThenInclude(p => p.Department)
                                                                .Include(w => w.Employee)
                                                                    .ToListAsync();
                    List<Employee> employees = new List<Employee>();
                    IdentityUser user = await _userManager.FindByNameAsync(_signInManager.Context.User.Identity.Name);
                    Employee employee = await _db.Employees
                                                .Where(e => e.IdentityUserId == user.Id)
                                                    .FirstOrDefaultAsync();
                    if (employee == null)
                    {
                        Work work = await _db.Works
                                                .Where(w => w.EmployeeId == employee.Id)
                                                    .Include(w => w.BranchPosition)
                                                         .ThenInclude(bp => bp.Position)
                                                             .ThenInclude(p => p.Department)
                                                                 .Include(w => w.Employee)
                                                                     .FirstOrDefaultAsync();
                        foreach (Work w in works)
                        {
                            if (w.BranchPosition.Position.DepartmentId == work.BranchPosition.Position.DepartmentId)
                            {
                                employees.Add(w.Employee);
                            }
                        }
                    }
                    else
                    {
                        employees = await _db.Employees.ToListAsync();
                    }
                    RoleModel rm = new RoleModel
                    {
                        Employees = employees,
                        Employee = employee,
                        Roles = await _db.Roles.ToListAsync()
                    };
                    return View(rm);
                }
                else
                {
                    await Initializer.InitialLayout("Employees", ViewBag, _db, _signInManager, "All Employees");
                    RoleModel rm = new RoleModel
                    {
                        Employees = await _db.Employees.ToListAsync(),
                        Roles = await _db.Roles.ToListAsync()
                    };
                    return View(rm);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> Index(RoleModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Admin"))
                {
                    await Initializer.InitialLayout("Employee", ViewBag, _db, _signInManager, "All Employees");
                    if (ModelState.IsValid)
                    {
                        IdentityUser identityUser = await _userManager.FindByEmailAsync(model.Email);
                        if (identityUser == null)
                        {
                            IdentityUser new_user = new IdentityUser
                            {
                                Email = model.Email,
                                UserName = model.UserName
                            };
                            IdentityResult UserCreateResult = await _userManager.CreateAsync(new_user, model.Password);
                            if (UserCreateResult.Succeeded)
                            {
                                IdentityResult RoleAddResult = await _userManager.AddToRoleAsync(new_user, model.Role);
                            }
                            await _db.SaveChangesAsync();
                            Employee Employee = await _db.Employees
                                                            .Where(e => e.Id == model.EmployeeId)
                                                                .FirstOrDefaultAsync();
                            Employee.IdentityUserId = (await _userManager.FindByEmailAsync(model.Email)).Id;
                            await _db.SaveChangesAsync();
                        }
                    }
                    List<Work> works = await _db.Works
                                                    .Include(w => w.BranchPosition)
                                                        .ThenInclude(bp => bp.Position)
                                                            .ThenInclude(p => p.Department)
                                                                .Include(w => w.Employee)
                                                                    .ToListAsync();
                    List<Employee> employees = new List<Employee>();
                    IdentityUser user = await _userManager.FindByNameAsync(_signInManager.Context.User.Identity.Name);
                    Employee employee = await _db.Employees
                                                .Where(e => e.IdentityUserId == user.Id)
                                                    .FirstOrDefaultAsync();
                    if (employee != null)
                    {
                        Work work = await _db.Works
                                                .Where(w => w.EmployeeId == employee.Id)
                                                    .Include(w => w.BranchPosition)
                                                         .ThenInclude(bp => bp.Position)
                                                             .ThenInclude(p => p.Department)
                                                                 .Include(w => w.Employee)
                                                                     .FirstOrDefaultAsync();
                        foreach (Work w in works)
                        {
                            if (w.BranchPosition.Position.DepartmentId == work.BranchPosition.Position.DepartmentId)
                            {
                                employees.Add(w.Employee);
                            }
                        }
                    }
                    else
                    {
                        employees = await _db.Employees.ToListAsync();
                    }
                    RoleModel rm = new RoleModel
                    {
                        Employees = employees,
                        Employee = employee,
                        Roles = await _db.Roles.ToListAsync()
                    };
                    return View(rm);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Employees", ViewBag, _db, _signInManager, "Add Employee");
                    return View();
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeModel employee)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Employees", ViewBag, _db, _signInManager, "Add Employee");
                    if (ModelState.IsValid)
                    {
                        if (employee.Photo != null)
                        {
                            string FileExt = employee.Photo.FileName.Substring(employee.Photo.FileName.LastIndexOf("."), employee.Photo.FileName.Length - employee.Photo.FileName.LastIndexOf("."));
                            string FileName = employee.Photo.FileName.Substring(0, employee.Photo.FileName.LastIndexOf(".")) + DateTime.Now.ToShortDateString().Replace("/", "") + FileExt;
                            string path = Path.Combine(_hosting.WebRootPath, "Employee", "Images", FileName);
                            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                            {
                                await employee.Photo.CopyToAsync(fs);
                            }
                            Employee e = await _db.Employees
                                                        .Where(emp => emp.IdentityNumber == employee.IdentityNumber)
                                                            .FirstOrDefaultAsync();
                            if (e == null)
                            {
                                Employee employ = new Employee()
                                {
                                    Name = employee.Name,
                                    Surname = employee.Surname,
                                    FatherName = employee.FatherName,
                                    BirthDate = Convert.ToDateTime(employee.BirthDate),
                                    Address = employee.Address,
                                    VillageRegister = employee.VillageRegister,
                                    IdentityNumber = employee.IdentityNumber,
                                    IdentityLastDay = Convert.ToDateTime(employee.IdentityLastDay),
                                    Education = employee.Education,
                                    FamilyStatus = employee.FamilyStatus,
                                    Gender = employee.Gender,
                                    PhotoLink = FileName,
                                    LastCalculate = DateTime.Now
                                };
                                await _db.Employees.AddAsync(employ);
                                await _db.SaveChangesAsync();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                ModelState.AddModelError("", "This Employee already exist");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Image is not correct");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Something is wrong");
                    }
                    return View(employee);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Employees", ViewBag, _db, _signInManager, "Edit");
                    Employee employee = _db.Employees.Where(e => e.Id == id).FirstOrDefault();
                    if (employee == null)
                    {
                        return BadRequest();
                    }
                    return View(employee);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Employees", ViewBag, _db, _signInManager, "Edit");
                    if (ModelState.IsValid)
                    {
                        Employee emp = _db.Employees.Where(e => e.Id == employee.Id).FirstOrDefault();
                        if (emp != null)
                        {
                            emp.Name = employee.Name;
                            emp.Surname = employee.Surname;
                            emp.FatherName = employee.FatherName;
                            emp.BirthDate = employee.BirthDate;
                            emp.Address = employee.Address;
                            emp.VillageRegister = employee.VillageRegister;
                            emp.IdentityNumber = employee.IdentityNumber;
                            emp.IdentityLastDay = employee.IdentityLastDay;
                            emp.Education = employee.Education;
                            emp.FamilyStatus = employee.FamilyStatus;
                            emp.Gender = employee.Gender;
                            emp.PhotoLink = employee.PhotoLink;
                            await _db.SaveChangesAsync();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Something is wrong");
                            return View(employee);
                        }
                    }
                    return View(employee);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        public async Task<JsonResult> GetPhotoLink()
        {
            IFormFile Photo = Request.Form.Files.FirstOrDefault();
            if (Photo != null)
            {
                string FileExt = Photo.FileName.Substring(Photo.FileName.LastIndexOf("."), Photo.FileName.Length - Photo.FileName.LastIndexOf("."));
                string filename = Photo.FileName.Substring(0, Photo.FileName.LastIndexOf(".")) + DateTime.Now.ToShortDateString().Replace("/", "") + FileExt;
                string path = Path.Combine(_hosting.WebRootPath, "Employee", "Images", filename);
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    await Photo.CopyToAsync(fs);
                }
                return Json(new { status = 200, data = filename });
            }
            else
            {
                Photo = Request.Form.Files.LastOrDefault();
                Photo = Request.Form.Files.LastOrDefault();
                string FileExt = Photo.FileName.Substring(Photo.FileName.LastIndexOf("."), Photo.FileName.Length - Photo.FileName.LastIndexOf("."));
                string filename = Photo.FileName.Substring(0, Photo.FileName.LastIndexOf(".")) + DateTime.Now.ToShortDateString().Replace("/", "") + FileExt;
                return Json(new { status = 400, data = filename });
            }
        }
        public JsonResult ChangeViewBag(int id)
        {
            if (id == 0)
            {
                return Json(new { status = 400, message = "Please select one of them" });
            }
            else
            {
                ViewBag.EmployeeId = id;
                return Json(new { status = 200 });
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            Employee employee = await _db.Employees.Where(e => e.Id == id).FirstOrDefaultAsync();
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Absent(int Month)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Employees", ViewBag, _db, _signInManager, "Employee Attendance");
                    DateTime LastCheckDate;
                    if (Month == 0 || Month == DateTime.Now.Month)
                    {
                        Month = DateTime.Now.Month;
                        LastCheckDate = DateTime.Now;
                    }
                    else
                    {
                        LastCheckDate = Convert.ToDateTime(Month + "/" + DateTime.DaysInMonth(DateTime.Now.Year, Convert.ToInt32(Month)) + "/" + DateTime.Now.Year);
                    }
                    List<Employee> employees = await _db.Employees.ToListAsync();
                    List<Absent> Absents = await _db.Absents.Where(a => a.From.Year == DateTime.Now.Year && a.From.Month == Month).ToListAsync();
                    List<Absent> annualAbsents = await _db.Absents.Where(a => a.From.Year == DateTime.Now.Year).ToListAsync();
                    AbsentModel model = new AbsentModel
                    {
                        Employees = employees,
                        Absents = Absents,
                        AnnualAbsents = annualAbsents,
                        LastCheckDate = LastCheckDate
                    };
                    return View(model);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
        [HttpPost]
        public async Task<IActionResult> Absent(AbsentModel model)
        {
            if (_signInManager.Context.User.Identity.IsAuthenticated)
            {
                if (HttpContext.User.IsInRole("Hr"))
                {
                    await Initializer.InitialLayout("Employees", ViewBag, _db, _signInManager, "Employee Attendance");
                    List<Employee> employees = await _db.Employees.ToListAsync();
                    List<Absent> Absents = await _db.Absents.Where(wc => wc.From.Year == DateTime.Now.Year && wc.From.Month == DateTime.Now.Month && wc.EmployeeId == model.EmployeeId).ToListAsync();
                    List<Absent> annualAbsents = await _db.Absents.Where(a => a.From.Year == DateTime.Now.Year && a.EmployeeId == model.EmployeeId).ToListAsync();
                    Employee employee = await _db.Employees
                                                    .Where(e => e.Id == model.EmployeeId)
                                                        .FirstOrDefaultAsync();
                    AbsentModel wcm = new AbsentModel();
                    if (ModelState.IsValid)
                    {
                        bool IsExist = false;
                        if (model.From == model.To)
                        {
                            ModelState.AddModelError("", "From and To must not be equal");
                        }
                        else if (model.From < employee.LastCalculate)
                        {
                            ModelState.AddModelError("", "You can't add this time arrange");
                        }
                        else
                        {
                            foreach (Absent absent in Absents)
                            {
                                if (absent.From <= model.From && absent.To >= model.From)
                                {
                                    IsExist = true;
                                    absent.From = model.From;
                                    absent.To = model.To;
                                    absent.PermissionRole = model.PermissionRole;
                                    absent.Reason = model.Reason;
                                    absent.EmployeeId = model.EmployeeId;
                                    if (absent.From.Day - absent.To.Day == 5)
                                    {
                                        Work work = await _db.Works
                                                                .Where(w => w.EmployeeId == absent.EmployeeId && w.LeaveTime == DateTime.MinValue)
                                                                    .FirstOrDefaultAsync();
                                        work.LeaveTime = absent.To;
                                        work.Reason = "5 gün və artıq işə gəlməməsi";
                                    }
                                    await _db.SaveChangesAsync();
                                    break;
                                }
                            }
                            if (!IsExist)
                            {
                                Absent absent = new Absent
                                {
                                    From = model.From,
                                    To = model.To,
                                    PermissionRole = model.PermissionRole,
                                    EmployeeId = model.EmployeeId,
                                    Reason = model.Reason
                                };
                                await _db.Absents.AddAsync(absent);
                                if (absent.From.Day - absent.To.Day == 5)
                                {
                                    Work work = await _db.Works
                                                            .Where(w => w.EmployeeId == absent.EmployeeId && w.LeaveTime == DateTime.MinValue)
                                                                .FirstOrDefaultAsync();
                                    work.LeaveTime = absent.To;
                                    work.Reason = "5 gün və artıq işə gəlməməsi";
                                }
                                await _db.SaveChangesAsync();
                            }
                        }
                        Absents = await _db.Absents.Where(wc => wc.From.Year == DateTime.Now.Year && wc.From.Month == DateTime.Now.Month && wc.EmployeeId == model.EmployeeId).ToListAsync();
                    }
                    else
                    {
                        wcm.From = model.From;
                        wcm.To = model.To;
                        wcm.PermissionRole = model.PermissionRole;
                        wcm.Reason = model.Reason;
                        wcm.EmployeeId = model.EmployeeId;
                        Absents = await _db.Absents.Where(wc => wc.From.Year == DateTime.Now.Year && wc.From.Month == DateTime.Now.Month && wc.EmployeeId == model.EmployeeId).ToListAsync();
                    }
                    wcm.Employees = employees;
                    wcm.Absents = Absents;
                    wcm.AnnualAbsents = annualAbsents;
                    wcm.LastCheckDate = DateTime.Now;
                    return View(wcm);
                }
            }
            return RedirectToAction("Forbidden", "Error");
        }
    }
}