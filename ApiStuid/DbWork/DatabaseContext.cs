using ApiStuid.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ApiStuid.DbWork
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<TaskResponsible> TaskResponsibles { get; set; }

        public DatabaseContext()
        {
            Database.EnsureCreated();
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
    }
}
