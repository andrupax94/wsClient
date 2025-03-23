using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using LoremNET;
using System.Collections.ObjectModel;
using Microsoft.VisualBasic;
public class SignalRService
{
    [Inject]
    private StatusMessageService _statusMessageService { get; set; }

    private IConfiguration _configuration;

    private HubConnection _connection;


    private int _mensajesRecibidos = 0;
    public string MensajesRecibidos
    {
        get => _mensajesRecibidos.ToString();
        set
        {
            if (_mensajesRecibidos.ToString() != value)
            {
                _mensajesRecibidos = int.Parse(value);
                OnMensajeRecibidosChanged();
            }
        }
    }

    public event Action MensajeRecibidosChanged;

    protected virtual void OnMensajeRecibidosChanged()
    {
        MensajeRecibidosChanged?.Invoke();
    }
    private int _mensajesEnviados = 0;
    public string MensajesEnviados
    {
        get => _mensajesEnviados.ToString();
        set
        {
            if (_mensajesEnviados.ToString() != value)
            {
                _mensajesEnviados = int.Parse(value);
                OnMensajeEnviadosChanged();
            }
        }
    }

    public event Action MensajeEnviadosChanged;

    protected virtual void OnMensajeEnviadosChanged()
    {
        MensajeEnviadosChanged?.Invoke();
    }

    private static string guid = "";
    public static string idother = "";
    public static string[] impresoras = { "Aun Sin Nada" };


    public List<Conexiones> conexiones = new List<Conexiones>();
    public event Action ConexionesChanged;
    public List<Conexiones> Conexiones
    {
        get => conexiones;
        set
        {
            if (conexiones != value)
            {
                conexiones = value;
                OnConexionesChanged();
            }
        }
    }

    protected virtual void OnConexionesChanged()
    {
        ConexionesChanged?.Invoke();
    }


    private static List<HubConnection> _connections = new List<HubConnection>();
    private int _clientCount = 20;
    private int _messageCount = 0;
    public bool bsw = false;
    public SignalRService(IConfiguration configuration, StatusMessageService statusMessageService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _statusMessageService = statusMessageService ?? throw new ArgumentNullException(nameof(statusMessageService));
    }
    public async Task Connect(bool limpiar = false, string serverHost = "https://localhost:7001/", string serverHub = "chathub", int clientCount = 1)
    {

        if (limpiar)
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

        }

        var numeroDeConexionesActuales = _connections.Count;
        Conexiones aux = new Conexiones { id = 0, IDConexion = "", MensajeEnviado = "", MensajeRecivido = "", Host = "" };
        for (int i = _connections.Count; i < clientCount + numeroDeConexionesActuales; i++)
        {

            var url = serverHost + serverHub;
            var connection = new HubConnectionBuilder()
               .WithUrl(url)
               .WithAutomaticReconnect()
               .Build();
            var conexionInfo = new Conexiones { id = i, IDConexion = connection.ConnectionId, MensajeEnviado = "", MensajeRecivido = "", Host = url };
            var localIndex = i;
            if (clientCount == 1)
            {
                aux = conexionInfo;
            }
            connection.On<string, string>("ReceiveMessage", (idconexion, message) =>
        {
            if (IsValidJson(message, out JsonDocument jsonDoc))
            {
                string instruccionElement = jsonDoc.RootElement.GetProperty("instruccion").GetString();
                string datosElement = jsonDoc.RootElement.GetProperty("datos").GetString();




                switch (instruccionElement)
                {
                    case "mensaje":

                        string datos = datosElement;
                        conexiones[localIndex].MensajeRecivido = datos;
                        Console.WriteLine($"📩 Mensaje recibido: {datos}");
                        break;
                    case "impresoras":

                        List<string> impresoras = JsonSerializer.Deserialize<List<string>>(datosElement);
                        conexiones[localIndex].Impresoras = impresoras;
                        Console.WriteLine($"📩 Mensaje recibido: {datosElement}");
                        break;
                }



            }
            else
            {
                conexiones[localIndex].MensajeRecivido = "El mensaje no es un JSON válido.";
                Console.WriteLine("El mensaje no es un JSON válido.");
            }

            _mensajesRecibidos++;
            OnMensajeRecibidosChanged();
            OnConexionesChanged();

        });
            await connection.StartAsync();
            conexionInfo.IDConexion = connection.ConnectionId;

            _connections.Add(connection);
            conexiones.Add(conexionInfo);
        }
        OnConexionesChanged();
        if (clientCount == 1)
        {
            _statusMessageService.StatusMessage = "Conexion exitosa, ID de conexion: " + aux.IDConexion;
        }
        else
            _statusMessageService.StatusMessage = "Conexiones creadas";
    }
    public async Task SendMessage(string mensajeEnviar = "", int origenId = 0, string destinoId = "0")
    {
        var origen = _connections[origenId];
        string destino;
        int number;
        bool isValidNumber = int.TryParse(destinoId, out number);
        if (isValidNumber)
        {
            destino = _connections[number].ConnectionId;
        }
        else
        {
            destino = destinoId;
        }


        if (origen?.State == HubConnectionState.Connected)
        {
            await origen.InvokeAsync("SendMessageToClient", destino, mensajeEnviar);

            _statusMessageService.StatusMessage = "Enviado: " + mensajeEnviar;
            conexiones[origenId].MensajeEnviado = mensajeEnviar;
            _mensajesEnviados++;
            OnMensajeEnviadosChanged();
            OnConexionesChanged();
        }
        else
        {
            _statusMessageService.StatusMessage = "No conectado a SignalR";
        }
    }

    // Funci�n para cerrar la conexi�n
    public async Task Close()
    {
        if (_connection?.State == HubConnectionState.Connected)
        {
            await _connection.StopAsync();
            _statusMessageService.StatusMessage = "Conexion cerrada";
        }
        else
        {
            _statusMessageService.StatusMessage = "No estaba conectado";
        }
    }
    public async Task CreateConecctions()
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
                // InvokeAsync(StateHasChanged);

            });
            await connection.StartAsync();
            conexionInfo.IDConexion = connection.ConnectionId;
            _connections.Add(connection);
            conexiones.Add(conexionInfo);
        }
        OnConexionesChanged();
        _statusMessageService.StatusMessage = "Conexiones creadas";
    }

    public async Task RunBenchmark()
    {
        if (_connections.Count == 0)
        {
            _statusMessageService.StatusMessage = "No hay conexiones";
        }
        else
        {
            bsw = !bsw;
            if (bsw)
            {
                _statusMessageService.StatusMessage = "benchmark en ejecucion";
            }
            else
            {
                _statusMessageService.StatusMessage = "benchmark detenido";
            }

            var random = new Random();
            int batchSize = 10; // Tamaño del lote
            int batchInterval = 500; // Intervalo entre envíos en milisegundos

            while (bsw)
            {
                // Dividir las conexiones en lotes
                var batches = _connections.Select((connection, index) => new { connection, index })
                                          .GroupBy(x => x.index / batchSize)
                                          .Select(g => g.Select(x => x.connection).ToList())
                                          .ToList();

                foreach (var batch in batches)
                {
                    var tasks = new List<Task>();

                    foreach (var connection in batch)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            var lorem = Lorem.Paragraph(5, 5);
                            var index = _connections.IndexOf(connection);
                            var message = index + ":" + lorem;
                            if (connection == batch.Last())
                                await connection.InvokeAsync("SendMessageToClient", _connections[0].ConnectionId, message);
                            else
                                await connection.InvokeAsync("SendMessageToClient", _connections[index + 1].ConnectionId, message);


                            conexiones[index].MensajeEnviado = message;
                            _messageCount++;

                            OnConexionesChanged();

                            // Esperar un tiempo aleatorio entre 1 y 200 ms
                            int delay = random.Next(1, 201);
                            await Task.Delay(delay);
                        }));
                    }

                    // Esperar a que todas las tareas del lote se completen
                    await Task.WhenAll(tasks);

                    // Esperar antes de enviar el siguiente lote
                    await Task.Delay(batchInterval);
                }

                // Añadir una breve pausa para evitar bloquear la UI
                await Task.Delay(50);
            }
        }
    }

    public async Task<List<string>> GetOtherConnectedClientsGuids(int conextionid = 0)
    {
        try
        {
            List<string> guids = await _connections[conextionid].InvokeAsync<List<string>>("GetOtherConnectedClientsGuids");
            return guids;
        }
        catch (Exception ex)
        {
            List<string> guid = [];
            return guid;

        }
    }
    private static bool IsValidJson(string message, out JsonDocument jsonDoc)
    {
        try
        {
            jsonDoc = JsonDocument.Parse(message);
            return jsonDoc.RootElement.TryGetProperty("instruccion", out _) &&
                   jsonDoc.RootElement.TryGetProperty("datos", out _);
        }
        catch
        {
            jsonDoc = null;
            return false;
        }
    }

}