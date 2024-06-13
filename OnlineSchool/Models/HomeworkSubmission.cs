using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineSchool.Models
{
    public class HomeworkSubmission
    {
        [Key]
        public int SubmissionId { get; set; }
        [ForeignKey("HomeworkId")]
        public int HomeworkId { get; set; }//идентификатор домашнего задания
        public Homework Homework { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }//идентификатор ученика
        public User User { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        public string File { get; set; }//название файла
        public DateTime SubmissionDate { get; set; }//дедлайн домашнего задания 
        [Required(ErrorMessage = "Обязательное поле")]
        public string CuratorComment { get; set; }//комменатрий куратора по проверенной работе
    }
}
