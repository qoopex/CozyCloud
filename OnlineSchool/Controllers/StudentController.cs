using Microsoft.AspNetCore.Mvc;
using OnlineSchool.Data;
using OnlineSchool.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace OnlineSchool.Controllers
{
    [Authorize(Roles = "Admin, Student")]
    public class StudentController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly OnlineSchoolDbContext _context;
        public StudentController(IWebHostEnvironment webHostEnvironment, OnlineSchoolDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public async Task<IActionResult> Courses()
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Fullname == User.Identity.Name);
            var studentCourses = await _context.UserGroups
                                           .Where(ug => ug.UserId == user.Id)
                                           .Include(ug => ug.Group)
                                                .ThenInclude(g => g.Subject)
                                                .ThenInclude(s => s.Groups.Where(gr => gr.UserGroups.Any(ug => ug.UserId == user.Id)))
                                                .ThenInclude(g => g.Lessons)
                                           .Select(ug => ug.Group.Subject)
                                           .Distinct()
                                           .ToListAsync();
            //var l = _context.Lessons.Where(l => l.GroupId == studentCourses[0].Groups.ElementAt(0).Id).SingleOrDefault();

            //await Console.Out.WriteLineAsync($"{user.Fullname} - {studentCourses[0].SubjectName} - {l.Topic}");
            return View(studentCourses);
        }
    }
}
