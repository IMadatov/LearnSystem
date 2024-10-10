using LearnSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearnSystem.DbContext;

[AllowAnonymous]
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{

    public DbSet<Student> Students { get; set; }

    public DbSet<Teacher> Teachers { get; set; }    

    public DbSet<Subject> Subjects { get; set; }

    public DbSet<Class> Classes { get; set; }

    public DbSet<Journal> Journales { get; set; }

    public DbSet<Lesson> Lessons { get; set; }

    public DbSet<StudentGrades> StudentGrades { get; set; }

    public DbSet<Roles> Roles {  get; set; }

    public DbSet<StatusUser> StatusUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .Entity<Student>()
            .HasMany(x => x.Grades)
            .WithOne(x => x.Student)
            .HasForeignKey(x => x.StudentId);

        builder
            .Entity<Lesson>()
            .HasMany(x => x.StudentGrades)
            .WithOne(x => x.Lesson)
            .HasForeignKey(x => x.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<User>()
            .Property(x => x.UserName)
            .IsRequired(false);

        builder
            .Entity<User>()
            .HasIndex(x => x.UserName)
            .IsUnique()
            .HasFilter($"{nameof(User.TelegramId)} is not null");

        builder.Entity<StatusUser>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        base.OnModelCreating(builder);
    }
    
}
