using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HRPayrollSystem.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Route("/[area]/[controller]/[action]/{id?}")]
    public class ErrorController : Controller
    {
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}