using Microsoft.EntityFrameworkCore;
using ECOS.Shared.Models;

namespace ECOS.Web.Data
{
    public class EcosDbContext : DbContext
    {
        public EcosDbContext(DbContextOptions<EcosDbContext> options) : base(options)
        {
        }

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Juez> Jueces { get; set; }
        public DbSet<Votacion> Votaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Votacion>()
                .HasOne(v => v.Juez)
                .WithMany(j => j.Votaciones)
                .HasForeignKey(v => v.JuezId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Votacion>()
                .HasOne(v => v.Equipo)
                .WithMany(e => e.Votaciones)
                .HasForeignKey(v => v.EquipoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Juez>()
                .HasIndex(j => j.Usuario)
                .IsUnique();
        }
    }
}