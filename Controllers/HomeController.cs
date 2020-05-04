using eLearning.Data;
using eLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace eLearning.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult TermsAndConditions()
        {
            return View();
        }

        public IActionResult MySubscriptions()
        {
            //Get current user ID
            var current_user_id = _userManager.GetUserId(User);

            //Query all subscriptions for this user
            var subscriptions = _context.UserCourse.Where(x => x.UserId == current_user_id).ToList();

            //Prepare list of the view model type
            var return_obj = new List<MySubscriptionsViewModel>();

            foreach(var s in subscriptions)
            {
                //Get the course name
                var course_name = _context.Course.Where(x => x.Id == s.CourseId).FirstOrDefault().Name;

                //Check Validity
                var license_validity = _context.LicenseKey.Where(x => x.Value == s.KeyUsed).FirstOrDefault().Active;

                var local_obj = new MySubscriptionsViewModel();

                local_obj.License_Key = s.KeyUsed;
                local_obj.Course_Name = course_name;
                local_obj.Valid = license_validity;

                return_obj.Add(local_obj);
            }

            return View(return_obj);
        }

        public IActionResult ReportBug()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
