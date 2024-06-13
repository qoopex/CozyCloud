using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.Models
{
    public class Lesson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(100)]
        public string Topic { get; set; }//Тема урока
        [Required(ErrorMessage = "Обязательное поле")]
        public DateTime Date { get; set; }//Дата проведения
        [Required(ErrorMessage = "Обязательное поле")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }//Время начала урока
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(255)]
        public string Link {  get; set; }//Ссылка на урок(стрим/запись стрима)
        [ForeignKey("SubjectId")]
        public int? GroupId { get; set; }   
        public Group Group { get; set; }
        public Homework Homework { get; set; }

    }
}
