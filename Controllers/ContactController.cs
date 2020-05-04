using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using eLearning.Data;
using eLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eLearning.Controllers
{
    [AllowAnonymous]
    public class ContactController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ContactViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Save in the database
                    _context.ContactMessages.Add(vm);
                    _context.SaveChanges();

                    ViewBag.Message = "Message received. We will get back to you shortly.";
                }
                catch (Exception ex)
                {
                    ModelState.Clear();
                    ViewBag.Message = $" Sorry we are facing a problem here {ex.Message}";
                }
            }

            return View();
        }
    }
}