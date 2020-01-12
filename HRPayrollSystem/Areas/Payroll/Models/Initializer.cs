using HRPayrollSystem.Areas.Admin.Models.Data.Models;
using HRPayrollSystem.Areas.Payroll.Models.Data.Payrole;
using HRPayrollSystem.Areas.Payroll.Models.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRPayrollSystem.Areas.Admin.Models
{
    public static class Initializer
    {
        public static async Task InitialLayout(string ActivePage,dynamic ViewBag,PayroleDbContext db, SignInManager<IdentityUser> _signInManager, string Action)
        {
            List<Page> Pages = await db.Pages.Include(p=>p.SubPages).ToListAsync();
            ViewBag.Pages = Pages;
            ViewBag.ActivePage = ActivePage;
            ViewBag.UserName = _signInManager.Context.User.Identity.Name;
            ViewBag.Controller = ActivePage;
            ViewBag.Action = Action;
        }
        public static async Task CreateBP(Branch branch, Position position, PayroleDbContext db)
        {
            if(branch == null)
            {
                if (position != null)
                {
                    List<Branch> branches = await db.Branchs.ToListAsync();
                    foreach (Branch b in branches)
                    {
                        BranchPosition bp = new BranchPosition
                        {
                            BranchId = b.Id,
                            PositionId = position.Id
                        };
                        await db.BranchPositions.AddAsync(bp);
                        await db.SaveChangesAsync();
                    }
                }
            }
            else
            {
                List<Position> positions = await db.Positions.ToListAsync();
                foreach (Position p in positions)
                {
                    BranchPosition bp = new BranchPosition
                    {
                        BranchId = branch.Id,
                        PositionId = p.Id
                    };
                    await db.BranchPositions.AddAsync(bp);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}