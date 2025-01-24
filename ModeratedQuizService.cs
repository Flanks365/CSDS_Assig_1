using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
namespace CSDS_Assign_1;
public class ModeratedQuizService
{
    private readonly List<SocketWithId> _sockets = new();

    public async Task HandleWebSocketConnection(WebSocket socket)
    {
        SocketWithId socketWithId = new SocketWithId(socket);
        _sockets.Add(socketWithId);
        var buffer = new byte[1024 * 2];
        while (socketWithId.Socket.State == WebSocketState.Open)
        {
            var result = await socketWithId.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socketWithId.Socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, default);
                break;
            }

            string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"ID: {socketWithId.Id}, Received message: {receivedMessage}");
            QuizData? quizData = JsonSerializer.Deserialize<QuizData>(receivedMessage);
            quizData.id = socketWithId.Id.ToString();
            string jsonString = JsonSerializer.Serialize(quizData);
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

            foreach (var s in _sockets)
            {
                await s.Socket.SendAsync(jsonBytes, WebSocketMessageType.Text, true, default);
            }
        }
        _sockets.Remove(socketWithId);
    }
}

public class SocketWithId
{
    public static int Count { get; set; }
    public int Id { get; set; }
    public WebSocket Socket { get; set; }
    public SocketWithId(WebSocket socket)
    {
        this.Socket = socket;
        this.Id = Count++;
    }
}

public class QuizData
{
    public string id { get; set; }
    public string dataType { get; set; }
    public string message { get; set; }
    public AnswerData[] answers {  get; set; }
    public string selection {  get; set; }

}

public class AnswerData
{
    public string id { get; set; }
    public string text { get; set; }
}
