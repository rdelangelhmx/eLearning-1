﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eLearning.Data;
using eLearning.Models;
using Microsoft.AspNetCore.Identity;

namespace eLearning.Controllers
{
    public class LecturesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LecturesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lectures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecture = await _context.Lecture
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }

            return View(lecture);
        }

        // GET: Lectures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lectures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Course_Id,Text_Content,Index,Lecture_Title,Lecture_Color")] Lecture lecture)
        {
            if (ModelState.IsValid)
            {
                lecture.Lecture_Color = "#3b5998";
                _context.Add(lecture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lecture);
        }

        // GET: Lectures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecture = await _context.Lecture.FindAsync(id);
            if (lecture == null)
            {
                return NotFound();
            }
            return View(lecture);
        }

        // POST: Lectures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Course_Id,Text_Content,Index,Lecture_Title,Owner_ID,Is_Zoom_Enabled,StartTime,Zoom_Invite_Link,Lecture_Color")] Lecture lecture)
        {
            if (id != lecture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lecture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LectureExists(lecture.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details","Trainings", new { id = lecture.Course_Id });
            }
            return View(lecture);
        }

        // POST: Lectures/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Id,Course_Id,Text_Content,Index,Lecture_Title,Lecture_Color")] Lecture lecture)
        {
            if (ModelState.IsValid)
            {
                lecture.Lecture_Color = "#3b5998";
                lecture.Owner_ID = _userManager.GetUserId(User);
                _context.Add(lecture);
                //Increment lecture counter in Training
                var train = _context.Training.Where(x => x.Course_Id == lecture.Course_Id).FirstOrDefault();
                train.No_Of_lectures++;
                _context.Training.Update(train);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details","Trainings", new { id = lecture.Course_Id} );
            }
            return View(lecture);
        }

        // POST: Lectures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner_id = _userManager.GetUserId(User);

            var lecture = await _context.Lecture.FindAsync(id);

            if (owner_id != lecture.Owner_ID && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Lecture.Remove(lecture);
            //Get the Training Item assigned to this course
            var train = _context.Training.Where(x => x.Course_Id == lecture.Course_Id).FirstOrDefault();
            train.No_Of_lectures--;
            _context.Training.Update(train);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details","Trainings", new { id = lecture.Course_Id });
        }

        private bool LectureExists(int id)
        {
            return _context.Lecture.Any(e => e.Id == id);
        }
    }
}
