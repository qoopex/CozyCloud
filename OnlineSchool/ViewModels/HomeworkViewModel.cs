using OnlineSchool.Models;

namespace OnlineSchool.ViewModels
{
    public class HomeworkViewModel
    {
        public IEnumerable <HomeworkSubmission> homeworkSubmissions { get; set; }
        public Group Group { get; set; }

    }
}
