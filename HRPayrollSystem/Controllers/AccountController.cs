using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPayrollSystem.Areas.Admin.Models;
using HRPayrollSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRPayrollSystem.Controllers
{
    public class AccountController : Controller
    {
        private PayroleDbContext _db;
        private UserManager<IdentityUser> Manager;
        private SignInManager<IdentityUser> SignInManager;
        public AccountController(PayroleDbContext db,
                                    UserManager<IdentityUser> manager,
                                        SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            Manager = manager;
            SignInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await Manager.FindByEmailAsync(loginModel.Email);
                if (user != null)
                {
                    var PasswordCheckResult = await SignInManager.CheckPasswordSignInAsync(user, loginModel.Password, false);
                    if (PasswordCheckResult.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, false);
                        return Redirect("/Payroll/Hr/Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email or Password is not valid");
                        return View(loginModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email or Password is not valid");
                    return View(loginModel);
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}