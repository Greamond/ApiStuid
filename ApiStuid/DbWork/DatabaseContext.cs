using ApiStuid.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static ApiStuid.Controllers.AuthController;

namespace ApiStuid.DbWork
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<ChapterTask> ChaptersTask { get; set; }
        public DbSet<ChapterSubtask> ChaptersSubtask { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<TaskResponsible> TaskResponsibles { get; set; }
        public DbSet<LoginResult> LoginResults => Set<LoginResult>();

        public DatabaseContext()
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    "server=localhost;database=stuid;uid=root;pwd=;",
                    new MySqlServerVersion(new Version(8, 0, 11))
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoginResult>().HasNoKey().ToView(null);
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);
        }
    }
}
