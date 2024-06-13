using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Models
{
    public class UserGroup
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }        
        public User User { get; set; }

        [ForeignKey("GroupId")]
        public int GroupId { get; set; }       
        public Group Group { get; set; }
        public UserGroup() { }
    }
}
