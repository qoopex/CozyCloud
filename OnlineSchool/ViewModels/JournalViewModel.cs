using OnlineSchool.Models;

namespace OnlineSchool.ViewModels
{
    public class JournalViewModel
    {
        public IEnumerable<Subject> Subjects { get; set; }
        public Lesson Lesson { get; set; }
        public IFormFile HomeworkFile {  get; set; }
    }
}
