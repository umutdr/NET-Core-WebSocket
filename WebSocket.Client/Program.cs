using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocket.Client
{
    class Program
    {
         static async Task Main(string[] args)
        {
            Console.WriteLine("Press enter to connect the web socket server");
            Console.ReadLine();

            using (ClientWebSocket clientWebSocket = new())
            {
                try
                {
                    await clientWebSocket.ConnectAsync(new Uri("ws://127.0.0.1:5000/ws"), CancellationToken.None);
                    var isConnectionOpen = IsConnectionOpen(clientWebSocket);
                    if (isConnectionOpen)
                    {
                        Console.WriteLine($"Connection Established at {DateTime.UtcNow:u} with state of \"{clientWebSocket.State}\"");
                        Console.WriteLine("You can send messages to web socket server... (Type a message and press enter key to send)");
                        while (isConnectionOpen)
                        {
                            string messageToSend = Console.ReadLine();
                            await SendMessage(clientWebSocket, $"{messageToSend}", WebSocketMessageType.Text, true, CancellationToken.None);
                            isConnectionOpen = IsConnectionOpen(clientWebSocket);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Connection Failed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n\nClient.Main Exception : {ex.Message}\n\n");
                }
            }

            Console.WriteLine("Disconnected. Press enter to exit...");
            Console.ReadLine();
        }

        private static bool IsConnectionOpen(ClientWebSocket clientWebSocket)
        {
            return clientWebSocket.State.Equals(WebSocketState.Open);
        }

        public async static Task<bool> SendMessage(System.Net.WebSockets.WebSocket webSocketClient, string message, WebSocketMessageType webSocketMessageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            bool result = false;

            try
            {
                var messageBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await webSocketClient.SendAsync(messageBuffer, webSocketMessageType, endOfMessage, cancellationToken);

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\nSendMessage Exception : {ex.Message}\n\n");
            }

            return result;
        }

    }
}
