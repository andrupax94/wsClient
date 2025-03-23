using Microsoft.AspNetCore.Components;
namespace prueba.Pages.Conectar

{
    public partial class Conectar : ComponentBase
    {


        [Inject]
        public SignalRService _signalRService { get; set; }

        private int _clientCount { get; set; } = 1;

        public string serverHost { get; set; } = "https://localhost:7001/";

        public string serverHub { get; set; } = "chathub";

    }
}
