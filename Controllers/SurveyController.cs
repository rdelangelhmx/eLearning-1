using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLearning.Data;
using eLearning.Migrations;
using eLearning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eLearning.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SurveyController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TakeSurvey()
        {
            //Mark in the database
            var user_id = _userManager.GetUserId(User);
            var obj = new Models.SurveyTaken
            {
                UserId = user_id
            };

            _context.SurveyTaken.Add(obj);
            _context.SaveChanges();

            
            //Open a new tab with the survey

            return Redirect("https://forms.gle/wvPLQUqnVAvLdFXAA");
        }
    }
}
