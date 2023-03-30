using System.Net;
using System.Net.Sockets;
using System.Text;

var port = 25565;

var ip = new IPEndPoint(IPAddress.Parse("192.168.100.100"), port);

var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

try{
    listenSocket.Bind(ip);

    listenSocket.Listen(10);

    Console.WriteLine("Server started.");

    while (true){
        var handler = listenSocket.Accept();

        var builder = new StringBuilder();

        var data = new byte[256];

        do{
            var bytes = handler.Receive(data);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
        } while (handler.Available > 0);
        
        Console.WriteLine($"О {DateTime.Now.ToShortTimeString()} від {handler.RemoteEndPoint} отримано рядок: {builder}");

        var message = "undefined";
        if (builder.ToString().ToLower().Contains("привіт")){
            message = "Привіт, клієнте!";
        }
        else if (builder.ToString().ToLower().Contains("до побачення")){
            message = "До побачення, клієнте!";
        }
        
        data = Encoding.Unicode.GetBytes(message);
        handler.Send(data);
        Console.WriteLine($"О {DateTime.Now.ToShortTimeString()} до {handler.RemoteEndPoint} відправлено рядок: {message}");
        
        handler.Shutdown(SocketShutdown.Both);
        handler.Close();
    }
}
catch (Exception e){
    Console.WriteLine(e.Message);
}