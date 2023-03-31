using NP_PW_Server_1;

var server = new AsyncServer("192.168.100.100", 1024);
server.Start();
Console.Read();

