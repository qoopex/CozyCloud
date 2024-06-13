using OnlineSchool.Models;

namespace OnlineSchool.ViewModels
{
    public class AdminPanelViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<GroupViewModel> GroupViewModels { get; set; }
        public Role Role {  get; set; }
        public Group Group { get; set; }
        //public IEnumerable<Lesson> Lessons { get; set; }
        public List<UserGroupViewModel> UserGroups { get; set; }
        //public Lesson Lesson { get; set; }
        public IEnumerable<Subject> Subjects { get; set; }
        public Subject Subject { get; set; }
    }
}
