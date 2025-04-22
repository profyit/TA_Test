using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementCommon.Models;

namespace TaskManagementDBLayer.Entities
{
    

    namespace TaskManagerApi.Data
    {
        public class TaskDbContext : DbContext
        {
            public TaskDbContext(DbContextOptions<TaskDbContext> options)
                : base(options)
            {
            }

            // הגדרת הטבלה במסד הנתונים שתתאים למודל TaskItem
            public DbSet<TaskItem> Tasks { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // ניתן להוסיף כאן הגדרות נוספות למודל אם נדרש
                base.OnModelCreating(modelBuilder);

                
                 modelBuilder.Entity<TaskItem>().HasData(
                     new TaskItem { Id = 1, Title = "Task 1", Description = "This is a  task 1", IsCompleted = false, CreatedDate = DateTime.UtcNow },
                     new TaskItem { Id = 2, Title = "Task 2", Description = "This is a  task 2", IsCompleted = true, CreatedDate = DateTime.UtcNow.AddDays(-5) }
                 );
            }
        }
    }
}
