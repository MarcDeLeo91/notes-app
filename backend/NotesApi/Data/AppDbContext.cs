using Microsoft.EntityFrameworkCore;
using NotesApi.Models;

namespace NotesApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ENTIDADES ACADÉMICAS
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<NotaAcademica> Notas { get; set; }

        // ENTIDADES DE AGENTE / AUTOMATIZACIÓN
        public DbSet<User> Users { get; set; }
        public DbSet<AgentTask> AgentTasks { get; set; }
        public DbSet<AgentAuditLog> AgentAuditLogs { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<Note> Notes { get; set; }  // Nota general (prompts, tareas)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relaciones
            modelBuilder.Entity<Profesor>()
                .HasMany(p => p.Cursos)
                .WithOne(c => c.Profesor)
                .HasForeignKey(c => c.ProfesorId);

            modelBuilder.Entity<Curso>()
                .HasMany(c => c.Inscripciones)
                .WithOne(i => i.Curso)
                .HasForeignKey(i => i.CursoId);

            modelBuilder.Entity<Estudiante>()
                .HasMany(e => e.Inscripciones)
                .WithOne(i => i.Estudiante)
                .HasForeignKey(i => i.EstudianteId);

            modelBuilder.Entity<Inscripcion>()
                .HasMany(i => i.Notas)
                .WithOne(n => n.Inscripcion)
                .HasForeignKey(n => n.InscripcionId);

            // Índices y restricciones
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            // Conversión simple de DateTime a UTC
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }
                }
            }
        }
    }
}
