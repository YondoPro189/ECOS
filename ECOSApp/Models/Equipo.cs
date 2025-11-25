using System.ComponentModel.DataAnnotations;

namespace ECOSApp.Models
{
    public class Equipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del equipo es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        public string? FotoUrl { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}