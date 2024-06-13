using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace OnlineSchool.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(100)]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(60)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(64)]
        public string Password { get; set; }
        public ICollection<Subject> Subjects { get; set; }//Поле для учителя с его учебными предметами

        [ForeignKey("RoleId")]
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public Group CuratedGroup { get; set; }//Поле для куратора, если он курирует группу
        public ICollection<UserGroup> UserGroups { get; set; }

        public ICollection<HomeworkSubmission> HomeworkSubmissions { get; set; }

        public User()
        {
            Subjects = new List<Subject>();
            UserGroups = new List<UserGroup>();
        }

        }
    }



