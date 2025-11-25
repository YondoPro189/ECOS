using System.ComponentModel.DataAnnotations;

namespace ECOSApp.Models
{
    public class Juez
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [StringLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string? FotoUrl { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}