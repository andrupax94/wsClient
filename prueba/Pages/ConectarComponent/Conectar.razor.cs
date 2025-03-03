using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using LoremNET;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace prueba.Pages.ConectarComponent

{
    public partial class Conectar : ComponentBase
    {
        public string _statusMessage = "Sin Info";

        private HubConnection _connection;
        private int currentCount = 0;
        private int _messageCount = 0;
        private int _clientCount { get; set; } = 1;
        public string mensaje = "";
        public string idother = "";
        public bool bsw = false;
        // Impresoras recibidas desde el servidor
        public string[] impresoras = { "Aun Sin Nada" };
        List<Conexiones> conexiones = new List<Conexiones>();

        private static List<HubConnection> _connections = new List<HubConnection>();
        private string conexiones_statusMessage = "Sin Info";
        [Inject]
        private IConfiguration Configuration { get; set; }
        [Inject]
        public SignalRService _signalRService { get; set; }

        private string guid;
        // Esta funci�n se ejecuta cuando se inicializa el componente

        public string serverHost { get; set; } = "https://localhost:7001/";


        public string serverHub { get; set; } = "chathub";

        private async Task Connect()
        {
            // URL del servidor SignalR (Aseg�rate de que sea correcta)
            //string serverUrl = "https://ws-eue5czgwbbbuard7.canadacentral-01.azurewebsites.net/chathub";
            //string serverUrl = "https://signalrsinservicio-d2hweefnhqh5cpb7.canadacentral-01.azurewebsites.net/chathub";
            string serverUrl = serverHost + serverHub;
            guid = Configuration["ApplicationSettings:GUID"];

            idother = Configuration["ApplicationSettings:GUIDD"];
            // Creando la conexi�n SignalR
            _connection = new HubConnectionBuilder()
                .WithUrl(serverUrl + $"?guid={guid}", options =>
                {
                    // Forzar uso de WebSockets

                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
                })
                .WithAutomaticReconnect()  // Habilita reconexiones autom�ticas
                .Build();

            // Manejar mensajes recibidos desde SignalR
            _connection.On<string>("ReceiveMessage", (message) =>
            {
                // Intentar deserializar como un arreglo de impresoras
                try
                {
                    var impresorasObj = JsonSerializer.Deserialize<string[]>(message);
                    if (impresorasObj != null)
                    {
                        impresoras = impresorasObj;  // Actualizar el listado de impresoras
                        _statusMessage = "Impresoras Importadas";
                    }
                    else
                    {
                        _statusMessage = $"Mensaje Recibido: {message}";
                    }
                }
                catch (JsonException ex)
                {
                    _statusMessage = $"Mensaje Recibido: {message}";
                }

                InvokeAsync(StateHasChanged);  // Actualiza la interfaz
            });

            try
            {
                // Inicia la conexi�n con SignalR
                await _connection.StartAsync();
                _statusMessage = "Conexion exitosa, ID de conexion: " + _connection.ConnectionId;

            }
            catch (Exception ex)
            {
                _statusMessage = $"Error al conectar con SignalR: {ex.Message}";
            }
        }
        // Funci�n para enviar un mensaje
        private async Task SendMessage(string mensajeEnviar = "")
        {
            if (string.IsNullOrEmpty(mensajeEnviar))
                mensajeEnviar = mensaje;

            if (_connection?.State == HubConnectionState.Connected)
            {




                await _connection.InvokeAsync("SendMessageToClient", idother, mensajeEnviar);


                _statusMessage = "Enviado: " + mensajeEnviar;
            }
            else
            {
                _statusMessage = "No conectado a SignalR";
            }
        }

        // Funci�n para cerrar la conexi�n
        private async Task Close()
        {
            if (_connection?.State == HubConnectionState.Connected)
            {
                await _connection.StopAsync();
                _statusMessage = "Conexion cerrada";
            }
            else
            {
                _statusMessage = "No estaba conectado";
            }
        }
        private async Task CreateConecctions()
        {
            if (_connections.Count > 0)
            {
                foreach (var connection in _connections)
                {
                    await connection.StopAsync();
                }
                _connections.Clear();
                conexiones.Clear();
            }
            for (int i = 0; i < _clientCount; i++)
            {
                var url = "https://localhost:7001/chathub";
                var connection = new HubConnectionBuilder()
                   .WithUrl(url)
                   .WithAutomaticReconnect()
                   .Build();
                var conexionInfo = new Conexiones { id = i, IDConexion = connection.ConnectionId, MensajeEnviado = "", MensajeRecivido = "" };
                var localIndex = i;
                connection.On<string>("ReceiveMessage", (message) =>
                {
                    conexiones[localIndex].MensajeRecivido = message;
                    InvokeAsync(StateHasChanged);

                });
                await connection.StartAsync();
                conexionInfo.IDConexion = connection.ConnectionId;
                _connections.Add(connection);
                conexiones.Add(conexionInfo);
            }
        }
        private async Task RunBenchmark()
        {
            if (_connections.Count == 0)
            {
                _statusMessage = "No hay conexiones";
            }
            else
            {
                bsw = !bsw;
                if (bsw)
                {
                    _statusMessage = "benchmark en ejecucion";
                }
                else
                {
                    _statusMessage = "benchmark detenido";
                }

                var random = new Random();

                while (bsw)
                {
                    var tasks = new List<Task>();

                    for (var i = 0; i < _connections.Count; i++)
                    {
                        var connection = _connections[i];
                        var localIndex = i;

                        tasks.Add(Task.Run(async () =>
                        {
                            var lorem = Lorem.Paragraph(5, 5);
                            var message = localIndex + ":" + lorem;
                            var timestamp = DateTime.Now.ToString("o");

                            if (localIndex == _connections.Count - 1)
                                await connection.InvokeAsync("SendMessageToClient", _connections[0].ConnectionId, message);
                            else
                                await connection.InvokeAsync("SendMessageToClient", _connections[localIndex + 1].ConnectionId, message);

                            conexiones[localIndex].MensajeEnviado = message;
                            _messageCount++;
                            _statusMessage = _messageCount.ToString();
                            await InvokeAsync(StateHasChanged);

                            // Esperar un tiempo aleatorio entre 1 y 200 ms
                            int delay = random.Next(1, 201);
                            await Task.Delay(delay);
                        }));
                    }

                    // No esperar a que las tareas se completen antes de continuar
                    // Eliminar tareas completadas de la lista
                    tasks.RemoveAll(t => t.IsCompleted);

                    // Añadir una breve pausa para evitar bloquear la UI
                    await Task.Delay(50);
                }
            }
        }




    }
}
