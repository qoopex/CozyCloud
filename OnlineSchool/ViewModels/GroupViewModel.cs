using OnlineSchool.Models;

namespace OnlineSchool.ViewModels
{
    public class GroupViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? CuratorId { get; set; }
        public User Curator { get; set; }
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }
        public List<SecondUserGroupViewModel> UserGroups { get; set; }
    }
}
