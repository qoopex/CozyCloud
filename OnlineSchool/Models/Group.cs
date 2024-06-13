using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(75)]
        public string GroupName { get; set; }

        [ForeignKey("SubjectId")]
        public int? SubjectId { get; set; }
        public Subject Subject { get; set; }
        [ForeignKey("UserId")]
        public int? UserId { get; set; }//Куратор группы
        public User User { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }//Список учеников группы
        public ICollection<Lesson> Lessons { get; set; }
        public Group() 
        {
            UserGroups = new List<UserGroup>();
            Lessons = new List<Lesson>();
        }       


    }
}
