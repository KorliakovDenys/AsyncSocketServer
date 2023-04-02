using System.Net;
using System.Text;

namespace NP_PW_Server_1;

class Program {
    public static async Task Main(string[] args) {
        await AsyncServer.StartListeningAsync();
    }
}