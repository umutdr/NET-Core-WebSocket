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
                    var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(60 * 3));
                    var cancellationToken = cancellationTokenSource.Token;

                    await clientWebSocket.ConnectAsync(new Uri("ws://localhost:5000/ws"), cancellationToken);
                    var isConnectionOpen = IsConnectionOpen(clientWebSocket);
                    if (isConnectionOpen)
                    {
                        Console.WriteLine($"Connection Established at {DateTime.UtcNow:u} with state of \"{clientWebSocket.State}\"");
                        Console.WriteLine("You can send messages to web socket server... (Type a message and press enter key to send)");
                        while (isConnectionOpen)
                        {
                            string messageToSend = Console.ReadLine();
                            await SendMessage(clientWebSocket, $"{messageToSend}", WebSocketMessageType.Text, true, cancellationToken);

                            var buffer = new byte[1024];
                            var arraySegmentBuffer = new ArraySegment<byte>(buffer);

                            while (true)
                            {
                                WebSocketReceiveResult receiveResult
                                    = await clientWebSocket.ReceiveAsync(arraySegmentBuffer, cancellationToken);
                                string message = GetTextByBuffer(buffer, 0, receiveResult.Count);
                                Console.WriteLine($"Server says : {message}");

                                if (receiveResult?.EndOfMessage == true)
                                    break;
                            }

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

        public async static Task<bool> SendMessage(System.Net.WebSockets.WebSocket webSocket, string message, WebSocketMessageType webSocketMessageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            bool result = false;

            try
            {
                var messageBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await webSocket.SendAsync(messageBuffer, webSocketMessageType, endOfMessage, cancellationToken);

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\nSendMessage Exception : {ex.Message}\n\n");
            }

            return result;
        }

        public static string GetTextByBuffer(byte[] buffer, int offset, int count)
        {
            string result = string.Empty;

            try
            {
                result = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, offset, count));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\nGetText Exception : {ex.Message}\n\n");
            }

            return result;
        }


    }
}
