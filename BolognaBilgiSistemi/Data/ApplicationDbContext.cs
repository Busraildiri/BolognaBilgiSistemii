using BolognaBilgiSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace BolognaBilgiSistemi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<FacultyMember> FacultyMembers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Administrator>()
                .HasOne(a => a.Department)
                .WithOne(d => d.Administrator)
                .HasForeignKey<Administrator>(a => a.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacultyMember>()
                .HasOne(f => f.Department)
                .WithMany(d => d.FacultyMembers)
                .HasForeignKey(f => f.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseAssignment>()
                .HasOne(ca => ca.Course)
                .WithMany(c => c.CourseAssignments)
                .HasForeignKey(ca => ca.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseAssignment>()
                .HasOne(ca => ca.FacultyMember)
                .WithMany(fm => fm.CourseAssignments)
                .HasForeignKey(ca => ca.FacultyMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<FacultyMember>()
            //    .Navigation(fm => fm.CourseAssignments)
            //    .UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
