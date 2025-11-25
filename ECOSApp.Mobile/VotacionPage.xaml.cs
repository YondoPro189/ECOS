using ECOSApp.Mobile.Models;
using ECOSApp.Mobile.Services;

namespace ECOSApp.Mobile
{
    public partial class VotacionPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly int _equipoId;
        private readonly string _nombreEquipo;
        private const string CLAVE_JUECES = "ECOSJueces";
        
        private List<Juez> _jueces = new();
        private Juez? _juezSeleccionado;
        private int _puntuacion = 0;

        public VotacionPage(int equipoId, string nombreEquipo)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _equipoId = equipoId;
            _nombreEquipo = nombreEquipo;
            
            NombreEquipoLabel.Text = $"Votando para: {nombreEquipo}";
            CrearEstrellas();
        }

        private void CrearEstrellas()
        {
            EstrellasLayout.Children.Clear();

            for (int i = 1; i <= 5; i++)
            {
                int puntos = i; // Capturar el valor en una variable local
                
                var estrella = new Label
                {
                    Text = "☆",
                    FontSize = 50,
                    TextColor = Color.FromRgb(203, 213, 224)
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => OnEstrellaTapped(puntos);
                estrella.GestureRecognizers.Add(tapGesture);

                EstrellasLayout.Children.Add(estrella);
            }
        }

        private void OnClaveChanged(object sender, TextChangedEventArgs e)
        {
            ClaveErrorLabel.IsVisible = false;
            
            if (e.NewTextValue == CLAVE_JUECES)
            {
                // Clave correcta
                ClaveEntry.IsEnabled = false;
                JuezFrame.IsVisible = true;
                CalificacionFrame.IsVisible = true;
                CargarJueces();
            }
            else if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length >= CLAVE_JUECES.Length)
            {
                // Clave incorrecta
                ClaveErrorLabel.IsVisible = true;
            }
        }

        private async void CargarJueces()
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                _jueces = await _apiService.ObtenerJuecesAsync();

                if (_jueces.Any())
                {
                    JuezPicker.ItemsSource = _jueces.Select(j => $"{j.Nombre} - {j.Especialidad}").ToList();
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron jueces registrados", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar jueces: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private void OnJuezSeleccionado(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < _jueces.Count)
            {
                _juezSeleccionado = _jueces[selectedIndex];
                VerificarFormularioCompleto();
            }
        }

        private void OnEstrellaTapped(int puntos)
        {
            _puntuacion = puntos;
            ActualizarEstrellas();
            ActualizarTextoCalificacion();
            VerificarFormularioCompleto();
        }

        private void ActualizarEstrellas()
        {
            for (int i = 0; i < EstrellasLayout.Children.Count; i++)
            {
                var estrella = (Label)EstrellasLayout.Children[i];
                
                if (i < _puntuacion)
                {
                    estrella.Text = "★";
                    estrella.TextColor = Color.FromRgb(251, 191, 36); // Amarillo dorado
                }
                else
                {
                    estrella.Text = "☆";
                    estrella.TextColor = Color.FromRgb(203, 213, 224); // Gris claro
                }
            }
        }

        private void ActualizarTextoCalificacion()
        {
            var textos = new Dictionary<int, string>
            {
                { 1, "⭐ Muy Deficiente" },
                { 2, "⭐⭐ Deficiente" },
                { 3, "⭐⭐⭐ Regular" },
                { 4, "⭐⭐⭐⭐ Bueno" },
                { 5, "⭐⭐⭐⭐⭐ Excelente" }
            };

            CalificacionTexto.Text = textos.ContainsKey(_puntuacion) 
                ? textos[_puntuacion] 
                : "Selecciona una calificación";
        }

        private void VerificarFormularioCompleto()
        {
            BtnEnviarVoto.IsEnabled = _juezSeleccionado != null && _puntuacion > 0;
        }

        private async void OnEnviarVotoClicked(object sender, EventArgs e)
        {
            if (_juezSeleccionado == null || _puntuacion == 0)
            {
                await DisplayAlert("Datos Incompletos", "Por favor completa todos los campos", "OK");
                return;
            }

            // Confirmar voto
            bool confirmar = await DisplayAlert(
                "Confirmar Voto",
                $"¿Confirmas tu voto de {_puntuacion} estrellas para {_nombreEquipo}?\n\nNo podrás modificarlo después.",
                "Sí, Enviar",
                "Cancelar"
            );

            if (!confirmar) return;

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                BtnEnviarVoto.IsEnabled = false;

                // Verificar si ya votó
                var yaVoto = await _apiService.YaVotoAsync(_equipoId, _juezSeleccionado.Id);

                if (yaVoto)
                {
                    await DisplayAlert("Voto Duplicado", "Este juez ya votó por este equipo", "OK");
                    return;
                }

                // Enviar votación
                var votacion = new VotacionRequest
                {
                    EquipoId = _equipoId,
                    JuezId = _juezSeleccionado.Id,
                    Puntuacion = _puntuacion
                };

                var resultado = await _apiService.RegistrarVotacionAsync(votacion);

                if (resultado.Success)
                {
                    await DisplayAlert(
                        "¡Voto Registrado! ✅",
                        $"Tu voto de {_puntuacion} estrellas ha sido registrado exitosamente.",
                        "OK"
                    );

                    // Regresar a la pantalla anterior
                    await Navigation.PopToRootAsync();
                }
                else
                {
                    await DisplayAlert("Error", resultado.Message, "OK");
                    BtnEnviarVoto.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al enviar voto: {ex.Message}", "OK");
                BtnEnviarVoto.IsEnabled = true;
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }
    }
}