using OnlineSchool.Models;

namespace OnlineSchool.ViewModels
{
    public class SecondUserGroupViewModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}
