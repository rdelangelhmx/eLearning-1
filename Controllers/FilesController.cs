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
using Microsoft.Extensions.Hosting;

namespace eLearning.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostEnvironment _env;

        public FilesController(ApplicationDbContext context, IHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = "Admin,Tutor")]
        public IActionResult UploadScreen(int lecture_id, int course_id)
        {
            var m = new CourseLectureViewModel
            {
                Course_Id = course_id,
                Lecture_Id = lecture_id
            };
            return View(m);
        }

        [Authorize(Roles = "Admin,Tutor")]
        [HttpPost("FileUpload")]
        public async Task<IActionResult> Upload(List<IFormFile> files, int course_id, int lecture_id)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    //Create the new filename
                    var new_filename = course_id.ToString() + "_" + lecture_id.ToString() + "_" + formFile.FileName;

                    var course_resource = new CourseResources
                    {
                        Original_Name = formFile.FileName,
                        Generated_Name = new_filename,
                        Course_Id = course_id,
                        Lecture_Id = lecture_id
                    };

                    _context.CourseResources.Add(course_resource);
                    _context.SaveChanges();

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Course_Resources", new_filename);

                    filePaths.Add(filePath);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return RedirectToAction("Details", "Trainings", new { id = course_id });
        }

        [Authorize(Roles = "Admin,Tutor")]
        public IActionResult Delete(string filename_on_server, string original_filename)
        {
            if (filename_on_server == null)
                return Content("filename not present");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Course_Resources", filename_on_server);

            //Delete file from server
            System.IO.File.Delete(path);

            //Remove database entry
            var obj = _context.CourseResources.Where(x => x.Generated_Name == filename_on_server).FirstOrDefault();
            _context.CourseResources.Remove(obj);
            _context.SaveChanges();


            return RedirectToAction("Details", "Trainings", new { id = obj.Course_Id });
        }

        [Authorize]
        public async Task<IActionResult> Download(string filename_on_server, string original_filename)
        {
            if (filename_on_server == null)
                return Content("filename not present");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Course_Resources", filename_on_server);

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