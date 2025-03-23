using System.Text.Json;
using LoremNET;
using Microsoft.AspNetCore.Components;
using misFunciones;
namespace prueba.Pages.Mensajes

{
    public partial class Mensajes : ComponentBase
    {


        [Inject]
        public SignalRService _signalRService { get; set; }
        [Inject]
        public StatusMessageService _statusMessage { get; set; }



        public string destino { get; set; } = "1";
        public string origen { get; set; } = "1";
        public List<string> _conexionesConGuid = [];
        public string mensaje { get; set; } = Lorem.Paragraph(5, 5);
        protected override async Task OnInitializedAsync()
        {
            if (_signalRService.conexiones.Count() == 0)
            {
                destino = "noconexion";
                origen = "noconexion";
            }
            else
            {
                _conexionesConGuid = await _signalRService.GetOtherConnectedClientsGuids();
                destino = _signalRService.conexiones[0].id.ToString();
                origen = _signalRService.conexiones[0].id.ToString();
            }

        }
        public async Task EnviarMensaje()
        {
            if (destino == "noconexion" || origen == "noconexion")
                _statusMessage.StatusMessage = "No Hay Conexiones";
            else
            {
                string mensajeString = misFunciones.Mensajes.GenerateMessaje(mensaje);
                await _signalRService.SendMessage(mensajeString, int.Parse(origen), destino);
            }
        }
    }
}
