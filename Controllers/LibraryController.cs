using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eLearning.Data;
using eLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eLearning.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Repository(string category)
        {
            switch (category)
            {
                case "blockchain":
                    //Get blockchain meterials list and return
                    ViewBag.Title = "Blockchain";
                    ViewBag.category = "blockchain";
                    return View();
                case "cyberdiplomacy":
                    //Get cyberdiplomacy materials and return
                    ViewBag.Title = "Cyberdiplomacy";
                    ViewBag.category = "cyberdiplomacy";
                    return View();
                case "cybersecurity":
                    ViewBag.Title = "Cybersecurity";
                    ViewBag.category = "cybersecurity";
                    //Get cybersecurity materials and return
                    return View();
                default:
                    return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files, string category)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    //Create the new filename
                    var new_filename = category.ToString() + "_" + category.ToString() + "_" + formFile.FileName;

                    var library_resource = new LibraryResource
                    {
                        Original_Name = formFile.FileName,
                        Generated_Name = new_filename,
                        Category = category
                    };

                    _context.LibraryResources.Add(library_resource);
                    _context.SaveChanges();

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Repository\\" + category, new_filename);

                    filePaths.Add(filePath);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await formFile.CopyToAsync(stream);
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return RedirectToAction("Repository", "Library", new { category = category });
        }

        [Authorize]
        public async Task<IActionResult> Download(string filename_on_server, string original_filename, string category)
        {
            if (filename_on_server == null)
                return Content("filename not present");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Repository\\" + category, filename_on_server);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), original_filename);
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
