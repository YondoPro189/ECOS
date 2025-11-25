using System.Net.Http.Json;
using ECOSApp.Mobile.Models;

namespace ECOSApp.Mobile.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://tu-servidor.com/api"; // Cambia esto por tu URL

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BASE_URL)
            };
        }

        // Obtener todos los equipos
        public async Task<List<EquipoDetalle>> ObtenerEquiposAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<EquipoDetalle>>>("equipos");
                return response?.Data ?? new List<EquipoDetalle>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener equipos: {ex.Message}");
                return new List<EquipoDetalle>();
            }
        }

        // Obtener detalle de un equipo
        public async Task<EquipoDetalle?> ObtenerEquipoDetalleAsync(int equipoId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<EquipoDetalle>>($"equipos/{equipoId}");
                return response?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener equipo: {ex.Message}");
                return null;
            }
        }

        // Obtener todos los jueces
        public async Task<List<Juez>> ObtenerJuecesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<Juez>>>("jueces");
                return response?.Data ?? new List<Juez>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener jueces: {ex.Message}");
                return new List<Juez>();
            }
        }

        // Registrar votación
        public async Task<ApiResponse<object>> RegistrarVotacionAsync(VotacionRequest votacion)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("votaciones", votacion);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<object>>() 
                           ?? new ApiResponse<object> { Success = false, Message = "Error al procesar respuesta" };
                }
                
                return new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = $"Error del servidor: {response.StatusCode}" 
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar votación: {ex.Message}");
                return new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = $"Error: {ex.Message}" 
                };
            }
        }

        // Verificar si un juez ya votó por un equipo
        public async Task<bool> YaVotoAsync(int equipoId, int juezId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<bool>>(
                    $"votaciones/ya-voto?equipoId={equipoId}&juezId={juezId}");
                return response?.Data ?? false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar voto: {ex.Message}");
                return false;
            }
        }
    }
}