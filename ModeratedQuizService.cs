using System.Net.WebSockets;
namespace CSDS_Assign_1;
public class ModeratedQuizService
{
    private readonly List<WebSocket> _sockets = new();

    public async Task HandleWebSocketConnection(WebSocket socket)
    {
        _sockets.Add(socket);
        var buffer = new byte[1024 * 2];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, default);
                break;
            }

            foreach (var s in _sockets)
            {
                Console.WriteLine(buffer);
                await s.SendAsync(buffer[..result.Count], WebSocketMessageType.Text, true, default);
            }
        }
        _sockets.Remove(socket);
    }
}
