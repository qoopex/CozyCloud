using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(75)]
        public string SubjectName { get; set; }
        [MaxLength(1250)]
        public string SubjectDescription { get; set; }
        public int? UserId { get; set; }//Поле для учителя по учебному предмету
        public User User { get; set; }

        public ICollection<Group> Groups { get; set; }

        public Subject()
        {
            Groups = new List<Group>();
        }

    }
}
