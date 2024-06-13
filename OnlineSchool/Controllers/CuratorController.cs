using Microsoft.AspNetCore.Mvc;
using OnlineSchool.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OnlineSchool.ViewModels;
namespace OnlineSchool.Controllers
{
    [Authorize(Roles = "Admin, Curator")]
    public class CuratorController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly OnlineSchoolDbContext _context;
        public CuratorController(IWebHostEnvironment webHostEnvironment, OnlineSchoolDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        public async Task<IActionResult> Homeworks()
        {
            var curator = await _context.Users.SingleOrDefaultAsync(u => u.Fullname == User.Identity.Name);  // Предполагаем, что куратор авторизован
            //var group = _context.Groups
            //                    .Include(g => g.UserGroups)
            //                    .ThenInclude(s => s.Homeworks)
            //                    .ThenInclude(h => h.AssignedHomework)
            //                    .FirstOrDefault(g => g.CuratorId == curatorId);

            var group = await _context.Groups
                //.Include(g => g.User)
                .Include(g => g.Lessons)
                    .ThenInclude(l => l.Homework)
                        .ThenInclude(h => h.HomeworkSubmissions)
                .Include(g => g.Subject)
                .Include(g => g.UserGroups)
                    .ThenInclude(ug => ug.User)
                .FirstOrDefaultAsync(g => g.UserId == curator.Id);

            if (group == null)
            {
                return NotFound("Группа не найдена.");
            }

            var viewModel = new HomeworkViewModel
            {
                Group = group,
                //StudentsHomework = group.Students.Select(s => new StudentHomeworkViewModel
                //{
                //    StudentId = s.Id,
                //    StudentName = s.Name,
                //    Homeworks = s.Homeworks.Select(h => new HomeworkStatusViewModel
                //    {
                //        HomeworkId = h.Id,
                //        HomeworkTitle = h.Title,
                //        AssignedHomeworkFilePath = h.AssignedHomework.FilePath,
                //        SubmittedHomeworkFilePath = h.SubmittedFilePath,
                //        Status = h.Status,
                //        Grade = h.Grade
                //    }).ToList()
                //}).ToList()
            };

            return View(viewModel);
            return View();
        }
    }
}
