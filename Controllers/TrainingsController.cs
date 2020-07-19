using eLearning.Data;
using eLearning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Controllers
{
    public class TrainingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TrainingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Trainings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Training
                .FirstOrDefaultAsync(m => m.Course_Id == id);
            if (training == null)
            {
                return NotFound();
            }

            if (training.No_Of_lectures != 0)
            {
                List<LectureContentViewModel> content = new List<LectureContentViewModel>();

                //Get all the Lecture objects
                var lectures = _context.Lecture.Where(x => x.Course_Id == id).ToList().OrderBy(x => x.Index);

                //Foreach lecture - get all of its files
                foreach (var lecture in lectures)
                {
                    var files = _context.CourseResources.Where(x => x.Lecture_Id == lecture.Id).ToList();
                    var video = _context.VideoCourseResources.Where(x => x.Lecture_Id == lecture.Id).ToList();
                    var obj = new LectureContentViewModel
                    {
                        Lecture = lecture,
                        CourseResources = files,
                        VideoResources = video
                    };
                    content.Add(obj);
                }

                //Get the course with this id and the image
                var course = _context.Course.Find(id);
                if(course.CourseImage != null)
                {
                    string imageBase64Data = Convert.ToBase64String(course.CourseImage);
                    string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);
                    ViewBag.ImageDataUrl = imageDataURL;
                }
                
                return View(content);
            }

            var dummy_lecture = new Lecture
            {
                Course_Id = id.GetValueOrDefault(),
                Text_Content = "dummy"
            };

            var dummy_list = new LectureContentViewModel
            {
                Lecture = dummy_lecture,
                CourseResources = null
            };

            var l = new List<LectureContentViewModel>();
            l.Add(dummy_list);
            return View(l);
        }

        // GET: Trainings/Configure/5
        public async Task<IActionResult> Configure(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Training
                .FirstOrDefaultAsync(m => m.Course_Id == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training.Course_Id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Configure(int course_id, int no_of_lectures)
        {
            //Create all the lecture objects and store them in the database
            for (int i = 0; i < no_of_lectures; i++)
            {
                var lecture = new Lecture
                {
                    Lecture_Color = "#3b5998",
                    Course_Id = course_id,
                    Text_Content = "Not setup yet",
                    Lecture_Title = "Lecture " + (i + 1).ToString(),
                    Index = (i + 1).ToString(),
                    Owner_ID = _userManager.GetUserId(User)
                };
                _context.Lecture.Add(lecture);
                _context.SaveChanges();
            }

            //Update the training object for the course
            var train = _context.Training.Where(x => x.Course_Id == course_id).FirstOrDefault();
            train.No_Of_lectures = no_of_lectures;
            _context.Training.Update(train);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = course_id });
        }

        // GET: Trainings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Training.FindAsync(id);
            if (training == null)
            {
                return NotFound();
            }
            return View(training);
        }

        public IActionResult ResetWarning(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(id);
        }

        [HttpPost, ActionName("ResetConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult ResetConfirmed(int? id)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Tutor"))
            {
                return NotFound();
            }

            if (id == null)
            {
                return NotFound();
            }

            //get all the lectures for this course
            var lectures = _context.Lecture.Where(x => x.Course_Id == id).ToList();
            _context.Lecture.RemoveRange(lectures);
            //get the training item linked to this course
            var training = _context.Training.Where(x => x.Course_Id == id).FirstOrDefault();
            training.No_Of_lectures = 0;
            _context.Training.Update(training);

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        // POST: Trainings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Course_Id,No_Of_lectures")] Training training)
        {
            if (id != training.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.Id))
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
            return View(training);
        }

        // POST: Trainings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var training = await _context.Training.FindAsync(id);
            _context.Training.Remove(training);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Training.Any(e => e.Id == id);
        }
    }
}
