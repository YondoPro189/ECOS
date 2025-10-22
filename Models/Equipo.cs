using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOS.Digital.Models
{
    public class Equipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; }
        
        [StringLength(50)]
        public string TecnologiaUsada { get; set; } // Ejemplo: ASP.NET Core, React
        
        public string? UrlDemo { get; set; } // Enlace al repositorio o demo

        // Clave Foránea para el Alumno Líder (asume que el creador es el líder)
        [Required]
        public string AlumnoLiderId { get; set; }

        // Propiedad de Navegación
        [ForeignKey("AlumnoLiderId")]
        public ApplicationUser? AlumnoLider { get; set; }

        // Propiedad de Navegación inversa
        public ICollection<Voto> VotosRecibidos { get; set; } = new List<Voto>();
    }
}