using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.Data;
using OnlineSchool.Models;
using OnlineSchool.ViewModels;
using System.Diagnostics;

namespace OnlineSchool.Controllers
{
    public class SchoolController : Controller
    {
        private readonly OnlineSchoolDbContext _context;

        public SchoolController(OnlineSchoolDbContext context)
        {
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
