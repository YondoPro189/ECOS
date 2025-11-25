using ECOSApp.Mobile.Models;
using ECOSApp.Mobile.Services;

namespace ECOSApp.Mobile
{
    public partial class EquipoDetallePage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly int _equipoId;
        private EquipoDetalle? _equipo;

        public EquipoDetallePage(int equipoId)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _equipoId = equipoId;
            
            // It's generally safer to call an async method without 'await' 
            // inside a constructor (or a method called from it) 
            // if it's purely for data loading and not critical to the construction flow.
            // But the current call is acceptable for data initialization.
            CargarDetalleEquipo(); 
        }

        private async void CargarDetalleEquipo()
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                BtnVotar.IsEnabled = false;

                _equipo = await _apiService.ObtenerEquipoDetalleAsync(_equipoId);

                if (_equipo != null)
                {
                    NombreLabel.Text = _equipo.Nombre;
                    DescripcionLabel.Text = _equipo.Descripcion;
                    PromedioLabel.Text = _equipo.PromedioVotos.ToString("0.0");
                    TotalVotosLabel.Text = _equipo.TotalVotos.ToString();
                }
                else
                {
                    // FIX: Changed DisplayAlert to DisplayAlertAsync
                    await DisplayAlertAsync("Error", "No se pudo cargar la información del equipo", "OK");
                }
            }
            catch (Exception ex)
            {
                // FIX: Changed DisplayAlert to DisplayAlertAsync
                await DisplayAlertAsync("Error", $"Error al cargar equipo: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                BtnVotar.IsEnabled = true;
            }
        }

        private async void OnVotarClicked(object sender, EventArgs e)
        {
            if (_equipo == null)
            {
                // Adding a check and alert in case _equipo is null on button click
                // FIX: Changed DisplayAlert to DisplayAlertAsync
                await DisplayAlertAsync("Error", "Los datos del equipo no están disponibles.", "OK");
                return;
            }

            await Navigation.PushAsync(new VotacionPage(_equipoId, _equipo.Nombre));
        }
    }
}