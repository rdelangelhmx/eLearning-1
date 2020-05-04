using eLearning.Data;
using eLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace eLearning.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Courses
        public IActionResult Index()
        {
            //If Admin - list all
            if (User.IsInRole("Admin"))
            {
                return View(_context.Course.ToList());
            }

            //Only show courses the user is registered
            //Get the list of courses
            var current_user_id = _userManager.GetUserId(User);
            var user_course_entries = _context.UserCourse.Where(c => c.UserId == current_user_id).ToList();
            List<Course> registered_courses = new List<Course>();

            foreach (var entry in user_course_entries)
            {
                var course = _context.Course.Where(c => c.Id == entry.CourseId).FirstOrDefault();
                //add course to vector
                if (!registered_courses.Contains(course))
                {
                    registered_courses.Add(course);
                }
            }

            return View(registered_courses);
        }

        // GET: Courses/NotRegistered
        public IActionResult NotRegistered()
        {
            return View();
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //Verify if user is registered at this course
            bool allowed = false;
            var current_user_id = _userManager.GetUserId(User);
            var user_course_entries = _context.UserCourse.Where(c => c.UserId == current_user_id).ToList();

            if (User.IsInRole("Admin"))
            {
                allowed = true;
            }

            foreach (var entry in user_course_entries)
            {
                //For each entry check key status
                var key_status = _context.LicenseKey.Where(x => x.Value == entry.KeyUsed).FirstOrDefault().Active;

                if (entry.CourseId == id && key_status == true)
                {
                    allowed = true;
                    break;
                }
            }

            if (allowed)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var course = await _context.Course.FirstOrDefaultAsync(m => m.Id == id);

                if (course == null)
                {
                    return NotFound();
                }

                return View(course);
            }
            else
            {
                return RedirectToAction("NotRegistered");
            }

        }

        public IActionResult Redeem()
        {
            return View();
        }

        public IActionResult RedeemSuccess()
        {
            return View();
        }

        public IActionResult RedeemFail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Redeem(string key)
        {
            try
            {
                if (key == null)
                    throw new Exception("Zero requested");

                var keys_in_database = _context.LicenseKey.ToList();

                foreach (var k in keys_in_database)
                {
                    if (k.Value == key && k.Used == false)
                    {
                        var current_user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        //Key found. Mark it as USED and allow access
                        k.Used = true;
                        _context.Update(k);
                        _context.SaveChanges();

                        var obj = new UserCourse
                        {
                            CourseId = k.Course_id,
                            UserId = current_user_id,
                            KeyUsed = k.Value
                        };

                        _context.UserCourse.Add(obj);
                        _context.SaveChanges();

                        return RedirectToAction("RedeemSuccess");
                    }
                }

                return RedirectToAction("RedeemFail");

            }
            catch
            {
                return RedirectToAction("RedeemFail");
            }

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateKey(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult GenerateKey(int course_id, int number)
        {
            try
            {
                if (number == 0)
                    throw new Exception("Zero requested");

                //Generate licenses
                string[] new_licenses = new string[number];
                for (int i = 0; i < number; i++)
                {
                    //Get current date
                    var current_date = DateTime.Now;
                    var plain_text = current_date + course_id.ToString() + i;
                    using (MD5 md5Hash = MD5.Create())
                    {
                        string hash = GetMd5Hash(md5Hash, plain_text);
                        string formatted_hash = FormatLicenseKey(hash);
                        new_licenses[i] = formatted_hash;
                        //Add in the database
                        var db_object = new LicenseKey
                        {
                            Value = formatted_hash,
                            Course_id = course_id,
                            Used = false,
                            Active = true
                        };
                        _context.LicenseKey.Add(db_object);
                        _context.SaveChanges();
                    }
                }
                return RedirectToAction("Success", "LicenseKeys", new { keys = new_licenses });

            }
            catch
            {
                return RedirectToAction("Error", "LicenseKeys");
            }

        }

        public string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        static string FormatLicenseKey(string productIdentifier)
        {
            productIdentifier = productIdentifier.Substring(0, 28).ToUpper();
            char[] serialArray = productIdentifier.ToCharArray();
            StringBuilder licenseKey = new StringBuilder();

            int j = 0;
            for (int i = 0; i < 28; i++)
            {
                for (j = i; j < 4 + i; j++)
                {
                    licenseKey.Append(serialArray[j]);
                }
                if (j == 28)
                {
                    break;
                }
                else
                {
                    i = (j) - 1;
                    licenseKey.Append("-");
                }
            }
            return licenseKey.ToString();
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,StartingDate,EndingDate,HasCertificate")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                //Get the inserted course
                var c = _context.Course.Where(x => x.Name == course.Name && x.StartingDate == course.StartingDate).FirstOrDefault();
                //Generate Training Item for course
                var train = new Training
                {
                    Course_Id = c.Id,
                    No_Of_lectures = 0
                };
                _context.Training.Add(train);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartingDate,EndingDate,HasCertificate")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
