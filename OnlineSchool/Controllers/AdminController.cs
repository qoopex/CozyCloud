using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineSchool.Data;
using OnlineSchool.Models;
using OnlineSchool.ViewModels;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace OnlineSchool.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly OnlineSchoolDbContext _context;

        public AdminController(OnlineSchoolDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AdminPanel()
        {
            var groups = await _context.Groups
            .Include(g => g.User) 
            .Include(g => g.Subject)
            .Include(g => g.UserGroups)
                .ThenInclude(ug => ug.User) 
            .ToListAsync();
            var groupViewModels = groups.Select(g => new GroupViewModel
            {
                GroupId = g.Id,
                GroupName = g.GroupName,
                Subject = g.Subject,
                SubjectId = g.SubjectId,
                CuratorId = g.UserId,
                Curator = g.User,
                UserGroups = g.UserGroups.Select(ug => new SecondUserGroupViewModel
                {
                    UserId = ug.UserId,
                    Fullname = ug.User.Fullname,
                    Email = ug.User.Email,
                    Role = ug.User.Role
                }).ToList()
            }).ToList();

            var userGroupViewModels = groups.Select(g => new UserGroupViewModel
            {
                Group = g,
                Users = g.UserGroups.Select(ug => ug.User).ToList()
            }).ToList();

            var subjects = await _context.Subjects.Include(s => s.Groups).ToListAsync();

            var adminPanelViewModel = new AdminPanelViewModel()
            {
                Roles = _context.Roles,
                Users = _context.Users,
                Groups = _context.Groups,
                Subjects = subjects,
                UserGroups = userGroupViewModels,
                GroupViewModels = groupViewModels
            };

            ViewBag.groupCount = adminPanelViewModel.Groups.Count();
            ViewBag.groups = adminPanelViewModel.Groups;

            List<User> Teachers = adminPanelViewModel.Users.Where(u => u.RoleId == 2).ToList();
            ViewBag.Teachers = Teachers;
            List<User> Curators = adminPanelViewModel.Users.Where(u => u.RoleId == 3).ToList();
            ViewBag.Curators = Curators;
            ViewBag.Subjects = adminPanelViewModel.Subjects;


			return View(adminPanelViewModel);
        }        
        [HttpPost]
        public async Task<IActionResult> AddStudentToGroup(int userId = -1, int groupId = -1)
        {
            if (userId != -1 && groupId != -1)
            {
                var existingUserGroup = await _context.UserGroups.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);
                if (existingUserGroup == null)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if(user.RoleId == 4)
                    {
                        UserGroup ug = new UserGroup();
                        ug.GroupId = groupId;
                        ug.UserId = userId;
                        await _context.UserGroups.AddAsync(ug);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        Console.WriteLine("ADMINCONTROLLER.ADDGROUPTOUSER ERROR: only student can be added to group!");
                    }
                }
                else
                {
                    Console.WriteLine("ADMINCONTROLLER.ADDGROUPTOUSER ERROR: usergroup is already exist!");
                }
            }
            else
            {
                Console.WriteLine("ADMINCONTROLLER.ADDGROUPTOUSER ERROR: groupid or userid is null!");
            }
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteStudentFromGroup(int userId = -1, int groupId = -1)
        {
            if (userId != -1 && groupId != -1)
            {
                var ug = await _context.UserGroups.FirstOrDefaultAsync(ug => ug.User.Id == userId && ug.Group.Id == groupId);
                if (ug != null)
                {
                   _context.UserGroups.Remove(ug);
                   await _context.SaveChangesAsync();
                }

            }
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(int userId, Role role)
        {
            var getRole = _context.Roles.FirstOrDefault(r => r.Name == role.Name);
            var selectedRole = role.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (selectedRole == "None")
            {
                user.Role = getRole;
            }

            else if (selectedRole == "Student")
            {
                user.Role = getRole;
            }

            else if (selectedRole == "Curator")
            {             
                user.Role = getRole;
            }
            else if (selectedRole == "Teacher")
            {
                user.Role = getRole;
            }
            else if (selectedRole == "Admin")
            {
                user.Role = getRole;
            }
            await _context.SaveChangesAsync();  
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveUser(int userId)
        {
            var user = await _context.Users
           .Include(u => u.Subjects)
           .Include(u => u.CuratedGroup)
           .Include(u => u.HomeworkSubmissions) 
           .Include(u => u.UserGroups)
           .FirstOrDefaultAsync(u => u.Id == userId);
            if(user != null)
            {
                if (user.CuratedGroup != null)
                {
                    user.CuratedGroup.UserId = null;
                    _context.Groups.Update(user.CuratedGroup);
                }
                if (user.Subjects != null)
                {
                    foreach (var subject in user.Subjects)
                    {
                        subject.UserId = null;
                        subject.User = null;
                    }
                }
                if (user.HomeworkSubmissions != null)
                {
                    _context.HomeworkSubmissions.RemoveRange(user.HomeworkSubmissions);
                }
                if (user.UserGroups != null)
                {
                    _context.UserGroups.RemoveRange(user.UserGroups);
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> CreateSubject(AdminPanelViewModel adminPanelViewModel)
        {
            if (adminPanelViewModel.Subject.SubjectName != null)
            {
                var Subject = adminPanelViewModel.Subject;
                await _context.Subjects.AddAsync(Subject);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPanel");
            }
            if (adminPanelViewModel.Subject.SubjectName == null)
            {
                Console.WriteLine("ADMINCONTROLLER.CREATESUBJECT ERROR: Subject is null!");
            }
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> AddTeacherToSubject(int userId = -1, int subjectId = -1)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject != null)
            {
                var teacher = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (teacher != null)
                {
                    if (subject.UserId == null)
                    {
                        subject.UserId = userId;
                        subject.User = teacher;
                        await _context.SaveChangesAsync();
                    }
                }               
            }          

            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTeacherFromSubject(int userId = -1, int subjectId = -1)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject != null)
            {
                subject.UserId = null;
                subject.User = null;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveSubject(int subjectId)
        {
            var subject = await _context.Subjects
            .Include(s => s.Groups)
                .ThenInclude(g => g.Lessons)
                    .ThenInclude(l => l.Homework)
                        .ThenInclude(h => h.HomeworkSubmissions)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
            {
                throw new ArgumentException("Subject not found");
            }

            if (subject.Groups != null)
            {
                foreach (var group in subject.Groups)
                {
                    if (group.Lessons != null)
                    {
                        foreach (var lesson in group.Lessons)
                        {
                            if (lesson.Homework != null)
                            {
                                if (lesson.Homework.HomeworkSubmissions != null)
                                {
                                    _context.HomeworkSubmissions.RemoveRange(lesson.Homework.HomeworkSubmissions);
                                }
                                _context.Homeworks.Remove(lesson.Homework);
                            }
                            _context.Lessons.Remove(lesson);
                        }
                    }
                    _context.Groups.Remove(group);
                }
            }

            _context.Subjects.Remove(subject);

            await _context.SaveChangesAsync();
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(AdminPanelViewModel adminPanelViewModel)
        {
            if (adminPanelViewModel.Group.GroupName != null)
            {
                var Group = adminPanelViewModel.Group;
                await _context.Groups.AddAsync(Group);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminPanel");
            }
            if (adminPanelViewModel.Group.GroupName == null)
            {
                Console.WriteLine("ADMINCONTROLLER.CREATEGROUP ERROR: Group is null!");
            }
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveGroup(int groupId)
        {
            var group = await _context.Groups
            .Include(g => g.UserGroups)
            .Include(g => g.Lessons)
                .ThenInclude(l => l.Homework)
                    .ThenInclude(h => h.HomeworkSubmissions)
            .FirstOrDefaultAsync(g => g.Id == groupId);
            if (group != null)
            {
                if (group.Lessons != null)
                {
                    foreach (var lesson in group.Lessons)
                    {
                        if (lesson.Homework != null)
                        {
                            if (lesson.Homework.HomeworkSubmissions != null)
                            {
                                _context.HomeworkSubmissions.RemoveRange(lesson.Homework.HomeworkSubmissions);
                            }
                            _context.Homeworks.Remove(lesson.Homework);                          
                        }
                        _context.Lessons.Remove(lesson);
                    }
                }

                if (group.UserGroups != null)
                {
                    _context.UserGroups.RemoveRange(group.UserGroups);
                }

                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> AddGroupToSubject(int groupId = -1, int subjectId = -1)
        {
            await Console.Out.WriteLineAsync(groupId + " " + subjectId);
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectId);
            if(group != null && subject != null)
            {
                subject.Groups.Add(group);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGroupFromSubject(int groupId = -1, int subjectId = -1)
        {
            if (subjectId != -1 && groupId != -1)
            {
                var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId && g.Subject.Id == subjectId);
                group.SubjectId = null;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> AddCuratorToGroup(int userId, int groupId)
        {
            var group = await _context.Groups
            .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group != null)
            {
                var curator = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

                if (curator != null)
                {
                    var existingGroupWithCurator = await _context.Groups
                        .FirstOrDefaultAsync(g => g.UserId == userId);

                    if (existingGroupWithCurator == null)
                    {

                        group.UserId = userId;
                        group.User = curator;

                        _context.Groups.Update(group);
                        await _context.SaveChangesAsync();
                    }
                }
            }
           
            return RedirectToAction("AdminPanel");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCuratorFromGroup(int userId = -1, int groupId = -1)
        {
            if (userId != -1 && groupId != -1)
            {
                var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId && g.User.Id == userId);
                group.UserId = null;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel");
        }    
    }
}
