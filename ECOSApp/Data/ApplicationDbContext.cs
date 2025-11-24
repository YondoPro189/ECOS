using Microsoft.EntityFrameworkCore;
using ECOSApp.Models;

namespace ECOSApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Juez> Jueces { get; set; }
        public DbSet<Votacion> Votaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Equipo
            modelBuilder.Entity<Equipo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).IsRequired();
                entity.Property(e => e.FotoUrl).HasMaxLength(500);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("datetime('now')");
            });

            // Configuración de Juez
            modelBuilder.Entity<Juez>(entity =>
            {
                entity.HasKey(j => j.Id);
                entity.Property(j => j.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(j => j.Especialidad).IsRequired().HasMaxLength(100);
                entity.Property(j => j.Descripcion).HasMaxLength(500);
                entity.Property(j => j.FotoUrl).HasMaxLength(500);
                entity.Property(j => j.FechaRegistro).HasDefaultValueSql("datetime('now')");
            });

            // Configuración de Votacion
            modelBuilder.Entity<Votacion>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.EquipoNombre).IsRequired().HasMaxLength(100);
                entity.Property(v => v.JuezNombre).IsRequired().HasMaxLength(100);
                entity.Property(v => v.Puntuacion).IsRequired();
                entity.Property(v => v.FechaVoto).HasDefaultValueSql("datetime('now')");

                // Índices para mejorar el rendimiento
                entity.HasIndex(v => v.EquipoId);
                entity.HasIndex(v => v.JuezId);
            });

            // Datos de ejemplo (seed data) - USANDO VALORES ESTÁTICOS
            var seedDate = new DateTime(2025, 11, 24, 11, 13, 49, 133, DateTimeKind.Utc);

            modelBuilder.Entity<Equipo>().HasData(
                new Equipo
                {
                    Id = 1,
                    Nombre = "ECOS-VOTE Innovación",
                    Descripcion = "Sistema de votación digital para eventos educativos con tecnología de vanguardia.",
                    FechaRegistro = seedDate
                },
                new Equipo
                {
                    Id = 2,
                    Nombre = "Tech Innovators",
                    Descripcion = "Plataforma de gestión empresarial con inteligencia artificial integrada.",
                    FechaRegistro = seedDate
                },
                new Equipo
                {
                    Id = 3,
                    Nombre = "Code Masters",
                    Descripcion = "Aplicación móvil para el seguimiento de proyectos colaborativos en tiempo real.",
                    FechaRegistro = seedDate
                }
            );

            modelBuilder.Entity<Juez>().HasData(
                new Juez
                {
                    Id = 1,
                    Nombre = "Dr. Carlos Méndez",
                    Especialidad = "Tecnología e Innovación",
                    Descripcion = "Especialista en desarrollo de software y metodologías ágiles con 15 años de experiencia.",
                    FechaRegistro = seedDate
                },
                new Juez
                {
                    Id = 2,
                    Nombre = "Dra. Ana García",
                    Especialidad = "Diseño UX/UI",
                    Descripcion = "Experta en experiencia de usuario y diseño de interfaces digitales centradas en el usuario.",
                    FechaRegistro = seedDate
                },
                new Juez
                {
                    Id = 3,
                    Nombre = "Ing. Roberto Sánchez",
                    Especialidad = "Arquitectura de Software",
                    Descripcion = "Arquitecto de soluciones en la nube con amplia experiencia en sistemas escalables.",
                    FechaRegistro = seedDate
                }
            );
        }
    }
}