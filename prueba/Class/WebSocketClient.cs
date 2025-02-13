using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketClient
{
    private ClientWebSocket _webSocket;
   
    public async Task ConnectAsync(string uri)
    {
        _webSocket = new ClientWebSocket();
        try
        {
            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.WriteLine("Conectado al servidor WebSocket");
    }

    public async Task SendMessageAsync(string message)
    {
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await _webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine($"Mensaje enviado: {message}");
    }

    public async Task<string> ReceiveMessageAsync()
    {
        byte[] buffer = new byte[1024];
        WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Mensaje recibido: {receivedMessage}");
        return receivedMessage;
    }

    public async Task CloseAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cierre del cliente", CancellationToken.None);
        Console.WriteLine("Conexión cerrada");
    }
}
