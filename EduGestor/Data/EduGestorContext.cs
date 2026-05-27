using Microsoft.EntityFrameworkCore;
using EduGestor.Models;
using EduGestor.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EduGestor.Data
{
    public class EduGestorContext : IdentityDbContext<AppUser>
    {
        public EduGestorContext(DbContextOptions<EduGestorContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = default!;  //Estudante
        public DbSet<StudentClass> StudentClasses { get; set; } = default!;   //Turma
        public DbSet<Teacher> Teachers { get; set; } = default!;   //Professor
        public DbSet<Discipline> Disciplines { get; set; } = default!;   //Disciplina
        public DbSet<Guardian> Guardians { get; set; } = default!;   //Responsavel
        public DbSet<Registration> Registrations { get; set; } = default!;   //Matricula
        public DbSet<Grade> Grades { get; set; } = default!;   //Nota
        public DbSet<DisciplineClass> DisciplineClasses { get; set; } = default!;   //Disciplina Turma
        public DbSet<Attendance> Attendances { get; set; } = default!; // Presença dos alunos

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // Student
            // =========================

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Guardian)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GuardianId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Registration
            // =========================

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Student)
                .WithMany(s => s.Registrations)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.StudentClass)
                .WithMany(c => c.Registrations)
                .HasForeignKey(r => r.StudentClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // DisciplineClass
            // =========================

            modelBuilder.Entity<DisciplineClass>()
                .HasOne(dc => dc.Teacher)
                .WithMany(t => t.DisciplineClasses)
                .HasForeignKey(dc => dc.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DisciplineClass>()
                .HasOne(dc => dc.Discipline)
                .WithMany(d => d.DisciplineClasses)
                .HasForeignKey(dc => dc.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DisciplineClass>()
                .HasOne(dc => dc.StudentClass)
                .WithMany(sc => sc.DisciplineClasses)
                .HasForeignKey(dc => dc.StudentClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Grade
            // =========================

            modelBuilder.Entity<Grade>()
                .Property(g => g.StudentGrade)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Registration)
                .WithMany(r => r.Grades)
                .HasForeignKey(g => g.RegistrationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.DisciplineClass)
                .WithMany(dc => dc.Grades)
                .HasForeignKey(g => g.DisciplineClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Guardian
            // =========================
            modelBuilder.Entity<Guardian>()
                .HasIndex(g => g.Cpf)
                .IsUnique();

            // ==========================
            // Attendance
            // ==========================
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Registration)
                .WithMany(r => r.Attendances)
                .HasForeignKey(a => a.RegistrationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.DisciplineClass)
                .WithMany(dc => dc.Attendances)
                .HasForeignKey(a => a.DisciplineClassId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
