using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using OnlineSchool.Data;
using OnlineSchool.Models;
using OnlineSchool.ViewModels;

namespace OnlineSchool.Controllers
{
    [Authorize(Roles = "Admin, Teacher")]
    public class TeacherController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly OnlineSchoolDbContext _context;
        public TeacherController(IWebHostEnvironment webHostEnvironment, OnlineSchoolDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public async Task <IActionResult> Journal()
        {
            var teacher = await _context.Users.SingleOrDefaultAsync(u => u.Fullname == User.Identity.Name); 
            var subjects = await _context.Subjects
                .Where(s => s.UserId == teacher.Id)
                    .Include(s => s.Groups)
                        .ThenInclude(g => g.Lessons)
                            .ThenInclude(l => l.Homework)
                    .ToListAsync();
            JournalViewModel journalViewModel = new JournalViewModel()
            {
                Subjects = subjects
            };
            return View(journalViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLesson(Lesson lesson, int subjectId, IFormFile homeworkFile)
        {
            string uniqueFileName = null;

            if (homeworkFile != null)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + homeworkFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await homeworkFile.CopyToAsync(fileStream);
                }
            }
            if(uniqueFileName != null)
            {
                lesson.Homework.FilePath = uniqueFileName != null ? "/uploads/" + uniqueFileName : null;
            }



            var subject = await _context.Subjects.Include(s => s.Groups).FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject != null)
            {
                var groups = subject.Groups;
                foreach (var group in groups)
                {                   
                    //lesson.GroupId = group.Id;
                    //lesson.Group = group;
                    //await Console.Out.WriteLineAsync(lesson.Group.GroupName + '-' + lesson.Group.Id);
                    //await _context.Lessons.AddAsync(lesson);
                    //await _context.Homeworks.AddAsync(lesson.Homework);

                    var newLesson = new Lesson
                    {
                        Topic = lesson.Topic,
                        StartTime = lesson.StartTime,
                        Link = lesson.Link,
                        Date = lesson.Date,
                        GroupId = group.Id,
                        Group = group
                    };

                    await _context.Lessons.AddAsync(newLesson);

                    var newHomework = new Homework
                    {
                        HomeworkName = lesson.Homework.HomeworkName,
                        Description = lesson.Homework.Description,
                        DueDate = lesson.Homework.DueDate,
                        FilePath = lesson.Homework.FilePath,
                        Lesson = newLesson
                    };

                    await _context.Homeworks.AddAsync(newHomework);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Journal");
        }

        [HttpPost]
        public async Task<IActionResult> EditLesson(string lessonId)
        {
            int lId = -1;
            int.TryParse(lessonId, out lId);
            if(lId != -1)
            {

            }
            return RedirectToAction("Journal");
        }
    }
}
