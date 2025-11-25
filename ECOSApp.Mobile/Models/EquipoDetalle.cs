namespace ECOSApp.Mobile.Models
{
    public class EquipoDetalle
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public List<Integrante> Integrantes { get; set; } = new();
        public double PromedioVotos { get; set; }
        public int TotalVotos { get; set; }
    }

    public class Integrante
    {
        public string Nombre { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string Rol { get; set; } = string.Empty;
    }

    public class Juez
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
    }

    public class VotacionRequest
    {
        public int EquipoId { get; set; }
        public int JuezId { get; set; }
        public int Puntuacion { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}