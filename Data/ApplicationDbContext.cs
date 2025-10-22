using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ECOS.Digital.Models;

namespace ECOS.Digital.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Voto> Votos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Equipo>()
                .HasOne(e => e.AlumnoLider)
                .WithMany()
                .HasForeignKey(e => e.AlumnoLiderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Equipo)
                .WithMany()
                .HasForeignKey(u => u.EquipoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Voto>()
                .HasOne(v => v.Equipo)
                .WithMany(e => e.VotosRecibidos)
                .HasForeignKey(v => v.EquipoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Voto>()
                .HasOne(v => v.UsuarioVotante)
                .WithMany(u => u.VotosEmitidos)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Voto>()
                .HasIndex(v => new { v.UserId, v.EquipoId })
                .IsUnique();
        }
    }
}