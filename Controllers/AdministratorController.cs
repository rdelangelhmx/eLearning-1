using eLearning.Data;
using eLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministratorController : Controller
    {
        private UserManager<IdentityUser> _userManager;

        private readonly ApplicationDbContext _context;

        public AdministratorController(eLearning.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Get
        public IActionResult ManageRolesSearchByEmail()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ManageRolesSearchByEmail(String email)
        {
            //Search for User by email
            try
            {
                var users = from u in _context.Users
                            select u;

                if (!string.IsNullOrEmpty(email))
                {
                    var user = users.Where(u => u.Email.Equals(email));
                    if (user.Count() == 1)
                    {
                        //Find user current role
                        var role = _context.UserRoles.Where(r => r.UserId == user.FirstOrDefault().Id);

                        if (role.Count() != 0)
                        {
                            var roleName = _context.Roles.Where(r => r.Id == role.FirstOrDefault().RoleId).FirstOrDefault().Name;
                            //Found a user with this email - show alter user role page
                            var alterUserModelObject = new AlterUserModel()
                            {
                                Email = user.FirstOrDefault().Email,
                                CurrentRole = roleName
                            };
                            return RedirectToAction("AlterUserRole", "Administrator", alterUserModelObject);
                        }
                        else
                        {
                            //Found a user with this email - show alter user role page
                            var alterUserModelObject = new AlterUserModel()
                            {
                                Email = user.FirstOrDefault().Email,
                                CurrentRole = "Student"
                            };
                            return RedirectToAction("AlterUserRole", "Administrator", alterUserModelObject);
                        }
                    }
                }

            }
            catch
            {
                return View();
            }

            return View();
        }

        //Get
        public IActionResult AlterUserRole(AlterUserModel user)
        {
            return View(user);
        }

        public async Task<IActionResult> ChangeRoleAsync(String email, String role)
        {
            var user = _context.Users.Where(u => u.Email.Equals(email)).FirstOrDefault();

            //Remove all entries for roles in database
            var previousRoles = _context.UserRoles.Where(u => u.UserId == user.Id);

            foreach (var previousRole in previousRoles)
            {
                _context.UserRoles.Remove(previousRole);
            }

            _context.SaveChanges();

            if (role != "Student")
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return RedirectToAction("ManageRolesSearchByEmail", "Administrator");
        }
    }
}