using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocket.Server.Helpers
{
    public static class WebSocketHelper
    {
        public async static Task AcceptTcpClientConnection(HttpContext httpContext)
        {
            //if (true)
            {
                Console.WriteLine("Checking if new connections allowed...");
            }

            using (System.Net.WebSockets.WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync())
            {
                Console.WriteLine($"Client with connection id {httpContext.Connection.Id} Connected at {DateTime.UtcNow:u}");
                await HandleTcpClientConnection(httpContext, webSocket);
            }
        }

        private async static Task HandleTcpClientConnection(HttpContext httpContext, System.Net.WebSockets.WebSocket webSocket)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    CancellationToken cancellationToken = cancellationTokenSource.Token;
                    var buffer = new byte[1024];
                    var arraySegmentBuffer = new ArraySegment<byte>(buffer);

                    Console.WriteLine($"Listening for messages...");
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(arraySegmentBuffer, cancellationToken);

                    if (receiveResult != null)
                    {
                        while (!receiveResult.CloseStatus.HasValue)
                        {
                            string clientMessage = GetTextByBuffer(buffer, 0, receiveResult.Count);
                            Console.WriteLine($"Client with connection id {httpContext.Connection.Id} says: {clientMessage}");

                            string messageToSend = $"Web Socket Server received your message at {DateTime.UtcNow:u}";
                            await SendMessage(webSocket, messageToSend, receiveResult.MessageType, receiveResult.EndOfMessage, cancellationToken);

                            receiveResult = await webSocket.ReceiveAsync(arraySegmentBuffer, cancellationToken);
                        }
                    }
                }
                catch (WebSocketException wsException)
                {
                    switch (wsException.WebSocketErrorCode)
                    {
                        case WebSocketError.Success:
                            break;
                        case WebSocketError.InvalidMessageType:
                            break;
                        case WebSocketError.Faulted:
                            break;
                        case WebSocketError.NativeError:
                            break;
                        case WebSocketError.NotAWebSocket:
                            break;
                        case WebSocketError.UnsupportedVersion:
                            break;
                        case WebSocketError.UnsupportedProtocol:
                            break;
                        case WebSocketError.HeaderError:
                            break;
                        case WebSocketError.ConnectionClosedPrematurely:
                            {
                                Console.WriteLine($"Client with connection id {httpContext.Connection.Id} disconnected unexpectedly.\nException Message: {wsException.Message}");
                                break;
                            }
                        case WebSocketError.InvalidState:
                            break;
                        default:
                            {
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n\nHandleTcpClientConnection Exception : {ex.Message}\n\n");
                }
            }
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
