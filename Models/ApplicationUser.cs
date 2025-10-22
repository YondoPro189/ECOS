using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECOS.Digital.Models
{
    // Extiende IdentityUser para agregar propiedades específicas del evento
    public class ApplicationUser : IdentityUser
    {
        // Propiedad para clasificar usuarios: "Alumno", "Maestro", "Invitado", "Admin"
        public string UserType { get; set; } = "Alumno"; // Valor por defecto

        // Clave Foránea a Equipo (NULLABLE: Maestros e Invitados no tienen Equipo)
        public int? EquipoId { get; set; }
        
        // Propiedad de Navegación a Equipo (el equipo al que pertenece el Alumno)
        [ForeignKey("EquipoId")]
        public Equipo? Equipo { get; set; }

        // Propiedad de Navegación inversa para saber qué Votos ha emitido el usuario
        public ICollection<Voto> VotosEmitidos { get; set; } = new List<Voto>();
    }
}