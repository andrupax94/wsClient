using System.Text.Json;
using LoremNET;
using Microsoft.AspNetCore.Components;
using misFunciones;
namespace prueba.Pages.Impresoras

{
    public partial class Impresoras : ComponentBase
    {


        [Inject]
        public SignalRService _signalRService { get; set; }
        [Inject]
        public StatusMessageService _statusMessage { get; set; }

        public List<string> impresoras = [];

        public string destino { get; set; } = "1";

        public string origen { get; set; } = "1";




        public string impresora { get; set; } = "";
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
                origen = _signalRService.conexiones[0].id.ToString();

                _conexionesConGuid = await _signalRService.GetOtherConnectedClientsGuids();
                if (_conexionesConGuid.Count > 0)
                    destino = _conexionesConGuid[0];

                await DameImpresoras();
                _signalRService.ConexionesChanged += StateHasChanged;
            }

        }
        public async void DameGuids(ChangeEventArgs e)
        {
            var value = "1";
            if (value != "noconexion")
            {
                var i = int.Parse(origen);

            }

        }
        public void Dispose()
        {
            _signalRService.ConexionesChanged -= StateHasChanged;
        }
        public async Task Imprimir()
        {
            var impresoraSelecionada = _signalRService.conexiones[int.Parse(origen)].Impresoras[int.Parse(impresora)];
            var json = misFunciones.Mensajes.GeneratePrinterRequest(mensaje, impresoraSelecionada);
            await _signalRService.SendMessage(json, int.Parse(origen), destino);
        }
        public async Task DameImpresoras()
        {
            if (destino == "noconexion" || origen == "noconexion")
            {
                _statusMessage.StatusMessage = "No Hay Conexiones";
                impresora = "";
            }
            else
            {
                string mensajeString = misFunciones.Mensajes.GeneratePrinterListRequest();

                await _signalRService.SendMessage(mensajeString, int.Parse(origen), destino);
                impresora = "0";
            }
        }
    }
}
