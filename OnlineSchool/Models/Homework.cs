using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Models
{
    public class Homework
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(50)]
        public string HomeworkName { get; set; }
        [MaxLength(50)]
        public string HomeworkDescription { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        public DateTime DueDate { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        public string FilePath { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public ICollection<HomeworkSubmission> HomeworkSubmissions { get; set; }//на одно общее дз у учеников будет много загрузок этого дз
    }
}
