using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NP_PW_Server_1;

internal class AsyncServer {
    private static readonly IPAddress LOCALHOST = IPAddress.Parse("127.0.0.1");

    private static byte[] _buffer = new byte[1024];

    public static async Task StartListeningAsync() {
        var ipAddress = IPAddress.TryParse("192.168.100.101", out var validatedIpAddress)
            ? validatedIpAddress
            : LOCALHOST;
        var port = 8888;

        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try {
            listener.Bind(new IPEndPoint(ipAddress, port));
            listener.Listen(10);

            Console.WriteLine($"Server started listening on {ipAddress}:{port}");

            while (true) {
                var handler = await listener.AcceptAsync();
                Console.WriteLine($"Accepted connection from {handler.RemoteEndPoint}");

                Task.Run(() => HandleRequestAsync(handler));
            }
        }
        catch (Exception exception) {
            Console.WriteLine($"Error occurred {exception.Message}");
        }
    }

    private static async Task HandleRequestAsync(Socket handler) {
        try {
            while (true) {
                var bytesRead = await handler.ReceiveAsync(new ArraySegment<byte>(_buffer), SocketFlags.None);
                if (bytesRead == 0) {
                    Console.WriteLine($"Connection closed by {handler.RemoteEndPoint}");
                }

                var request = Encoding.Unicode.GetString(_buffer, 0, bytesRead);
                Console.WriteLine($"Receive data from {handler.RemoteEndPoint}: {request}");

                var message = request.ToLower() switch {
                    "time" => DateTime.Now.ToShortTimeString(),
                    "date" => DateTime.Now.ToLongDateString(),
                    _ => "undefined"
                };

                var response = Encoding.Unicode.GetBytes(message);
                await handler.SendAsync(new ArraySegment<byte>(response, 0, response.Length), SocketFlags.None);
            }
        }
        catch (Exception exception) {
            Console.WriteLine($"Error occurred: {exception.Message}");
        }
        finally {
            handler.Close();
        }
    }
}