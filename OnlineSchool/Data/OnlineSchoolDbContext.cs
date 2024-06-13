
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using OnlineSchool.Models;

namespace OnlineSchool.Data
{
    public class OnlineSchoolDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<HomeworkSubmission> HomeworkSubmissions { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        public OnlineSchoolDbContext(DbContextOptions<OnlineSchoolDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Teacher" },
                new Role { Id = 3, Name = "Curator" },
                new Role { Id = 4, Name = "Student" },
                new Role { Id = 5, Name = "None" }
            );

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subjects)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.UserId)
                .IsUnique(false);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Subject)
                .WithMany(s => s.Groups)
                .HasForeignKey(g => g.SubjectId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(sg => sg.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(sg => sg.UserId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(sg => sg.Group)
                .WithMany(g => g.UserGroups)
                .HasForeignKey(sg => sg.GroupId);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Group)
                .WithMany(g => g.Lessons)
                .HasForeignKey(l => l.GroupId);

            modelBuilder.Entity<Homework>()
                .HasOne(h => h.Lesson)
                .WithOne(l => l.Homework)
                .HasForeignKey<Homework>(h => h.LessonId)
                .IsRequired(false);

            modelBuilder.Entity<HomeworkSubmission>()
                .HasOne(hs => hs.Homework)
                .WithMany(h => h.HomeworkSubmissions)
                .HasForeignKey(hs => hs.HomeworkId)
                .IsRequired(false);

            modelBuilder.Entity<HomeworkSubmission>()
                .HasOne(hs => hs.User)
                .WithMany(u => u.HomeworkSubmissions)
                .HasForeignKey(hs => hs.UserId);
    



            base.OnModelCreating(modelBuilder);
        }
    }
}
