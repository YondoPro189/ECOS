namespace ECOSApp.Models
{
    public class Votacion
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public string EquipoNombre { get; set; } = string.Empty;
        public int JuezId { get; set; }
        public string JuezNombre { get; set; } = string.Empty;
        public int Puntuacion { get; set; }
        public DateTime FechaVoto { get; set; } = DateTime.Now;
    }
}