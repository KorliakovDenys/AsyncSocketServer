using System.Net;
using System.Net.Sockets;

namespace NP_PW_Server_1;

internal class AsyncServer{
    private readonly IPEndPoint _endPoint;
    private Socket _socket;


    private static void AcceptCallBack(IAsyncResult asyncResult){
        if (asyncResult.AsyncState is not Socket socket) return;

        var ns = socket.EndAccept(asyncResult);

        Console.WriteLine(ns.RemoteEndPoint);

        var sendBuffer = System.Text.Encoding.Unicode.GetBytes(DateTime.Now.ToShortTimeString());

        ns.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendCallBack, ns);

        socket.BeginAccept(AcceptCallBack, socket);
    }

    private static void SendCallBack(IAsyncResult asyncResult){
        if (asyncResult.AsyncState is not Socket socket) return;

        socket.EndSend(asyncResult);
        socket.Shutdown(SocketShutdown.Send);
        socket.Close();
    }

    public AsyncServer(string ip, int port){
        _endPoint = IPAddress.TryParse(ip, out var validatedIpAddress)
            ? new IPEndPoint(validatedIpAddress, port)
            : new IPEndPoint(IPAddress.Any, port);
    }

    public void Start(){
        if (_socket != null) return;

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(_endPoint);
        _socket.Listen(10);
        _socket.BeginAccept(AcceptCallBack, _socket);
    }
}

//
// private readonly IPEndPoint _endP;
// private Socket _socket;
//
// public AsyncServer(string strAddr, int port){
//     _endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
// }
//
// private static void MyAcceptCallbackFunction(IAsyncResult ia){
//     var socket = (Socket)ia.AsyncState;
//     var ns = socket?.EndAccept(ia);
//
//     Console.WriteLine(ns.RemoteEndPoint.ToString());
//
//
//     var sendBuffer =
//         System.Text.Encoding.ASCII.GetBytes(DateTime.Now.ToString());
//
//     ns.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, MySendCallbackFunction, ns);
//
//     socket.BeginAccept(MyAcceptCallbackFunction, socket);
// }
//
// private static void MySendCallbackFunction(IAsyncResult ia){
//     var ns = (Socket)ia.AsyncState;
//     var n = ns.EndSend(ia);
//     ns.Shutdown(SocketShutdown.Send);
//     ns.Close();
// }
//
// public void StartServer(){
//     if (_socket != null)
//         return;
//     _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
//     _socket.Bind(_endP);
//     _socket.Listen(10);
//     _socket.BeginAccept(MyAcceptCallbackFunction, _socket);
// }