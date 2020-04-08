using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eLearning.Data;
using eLearning.Models;

namespace eLearning.Controllers
{
    public class LicenseKeysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LicenseKeysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LicenseKeys
        public async Task<IActionResult> Index()
        {
            return View(await _context.LicenseKey.ToListAsync());
        }

        // GET: LicenseKeys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseKey = await _context.LicenseKey
                .FirstOrDefaultAsync(m => m.Id == id);
            if (licenseKey == null)
            {
                return NotFound();
            }

            return View(licenseKey);
        }

        public IActionResult Success(string[] keys)
        {
            return View(keys);
        }

        public IActionResult Error()
        {
            return View();
        }

        // GET: LicenseKeys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseKey = await _context.LicenseKey
                .FirstOrDefaultAsync(m => m.Id == id);
            if (licenseKey == null)
            {
                return NotFound();
            }

            return View(licenseKey);
        }

        // POST: LicenseKeys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var licenseKey = await _context.LicenseKey.FindAsync(id);
            _context.LicenseKey.Remove(licenseKey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseKeyExists(int id)
        {
            return _context.LicenseKey.Any(e => e.Id == id);
        }
    }
}
