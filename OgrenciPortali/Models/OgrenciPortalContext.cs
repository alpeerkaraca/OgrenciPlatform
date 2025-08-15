using System.Data.Entity;

namespace OgrenciPortali.Models
{
    public class OgrenciPortalContext : DbContext
    {
        public OgrenciPortalContext() : base("name=OgrenciPortalContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<OfferedCourse> OfferedCourses { get; set; } 
        public DbSet<StudentCourse> StudentCourses { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .HasMany(u => u.AdvisedStudents)
                .WithOptional(u => u.Advisor)
                .HasForeignKey(u => u.AdvisorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.OfferedCourseId });

            base.OnModelCreating(modelBuilder);
        }
    }
}