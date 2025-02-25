using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketClient
{
    private ClientWebSocket _webSocket;
    public event Action<string> OnMessageReceived;
    public async Task ConnectAsync(string uri)
    {
        _webSocket = new ClientWebSocket();
       
        await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        _ = ListenForMessagesAsync();
        Console.WriteLine("Conectado al servidor WebSocket");
    }

    public async Task SendMessageAsync(string message)
    {
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await _webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine($"Mensaje enviado: {message}");
    }

    private async Task ListenForMessagesAsync()
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                OnMessageReceived?.Invoke(receivedMessage);
                Console.WriteLine($"Mensaje recibido: {receivedMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al recibir mensajes: {ex.Message}");
        }
    }

    public async Task CloseAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cierre del cliente", CancellationToken.None);
        Console.WriteLine("Conexión cerrada");
    }
}
