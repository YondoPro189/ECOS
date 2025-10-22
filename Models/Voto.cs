using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOS.Digital.Models
{
    public class Voto
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "La puntuación debe ser entre 1 y 5.")]
        public int Puntuacion { get; set; } // Escala de 1 a 5

        public DateTime FechaVoto { get; set; } = DateTime.Now;

        // --- Claves Foráneas ---

        // El equipo que recibe el voto
        public int EquipoId { get; set; }
        [ForeignKey("EquipoId")]
        public Equipo? Equipo { get; set; }

        // El usuario que emite el voto (Maestro o Invitado)
        public string UserId { get; set; } 
        [ForeignKey("UserId")]
        public ApplicationUser? UsuarioVotante { get; set; }
    }
}