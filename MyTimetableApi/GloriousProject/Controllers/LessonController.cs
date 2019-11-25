using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GloriousProject.Models;
using System.Net;
using System.IO;
using SimpleJSON;
using GloriousProject.Services;

namespace GloriousProject.Controllers
{
    [Route("api/[controller]")]
    public class LessonController : Controller
    {
        private readonly EntityFrameworkDbContext _context;
        private readonly Timetable _timetable;
        public LessonController(EntityFrameworkDbContext context, Timetable timetable)
        {
            _context = context;
            _timetable = timetable;
        }
        [HttpGet]
        public List<Lesson> Get()
        {
            DateTime startDay = DateTime.Now.AddDays(-((int)DateTime.Today.DayOfWeek) + 1);
            List<Lesson> lessons = Mapper.Mapper.RuzLessonToLesson(
                _timetable.GetLessons(startDay, startDay.AddDays(6), "rvmaryatkin@edu.hse.ru"));
            List<Lesson> deleted = _context.Lessons.Where(l => l.Status == "Delete").ToList();
            foreach (var les in lessons.ToList())
            {
                foreach (var del in deleted)
                {
                    if (del == les)
                        lessons.Remove(les);
                }
            }
            lessons.AddRange(_context.Lessons.Where(l => l.Status == "Add"));
            return lessons.OrderBy(x => x.Date).ThenBy(x => x.EndLesson).ToList();
        }
        [HttpPost]
        public IActionResult Post([FromBody] Lesson item)
        {
            var result = _context.Lessons.Add(item);
            _context.SaveChanges();
            return Ok(result.Entity);
        }
        [HttpPut]
        public IActionResult Put([FromBody] Lesson item)
        {
            _context.Lessons.Update(item);
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (id == null)
                return BadRequest();
            Lesson lesson = _context.Lessons.Find(id);
            _context.Lessons.Remove(lesson);
            _context.SaveChanges();
            return Ok();
        }
    }
}
