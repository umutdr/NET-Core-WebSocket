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
            Console.WriteLine("Press enter to connect to the webSocket...");
            Console.ReadLine();

            using (ClientWebSocket client = new ClientWebSocket())
            {
                Uri serverUri = new Uri("ws://127.0.0.1:5000/send");

                try
                {
                    await client.ConnectAsync(serverUri, CancellationToken.None);
                    while (client.State == WebSocketState.Open)
                    {
                        Console.WriteLine("Type your message and press enter to send...");
                        string msg = Console.ReadLine();

                        if (!string.IsNullOrEmpty(msg))
                        {
                            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
                            var responseBuffer = new byte[1024];
                            var offset = 0;
                            var packet = 1024;
                            while (true)
                            {
                                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                                WebSocketReceiveResult response = await client.ReceiveAsync(bytesReceived, CancellationToken.None);
                                var responseMsg = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                                Console.WriteLine(responseMsg);

                                if (response.EndOfMessage)
                                    break;
                            }
                        }
                    }
                }
                catch (WebSocketException wsException)
                {
                    Console.WriteLine(wsException.Message);
                }
            }

            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }
}
