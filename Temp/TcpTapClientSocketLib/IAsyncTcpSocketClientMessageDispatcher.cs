using System.Threading.Tasks;

namespace TcpTapClientSocketLib
{
    public interface IAsyncTcpSocketClientMessageDispatcher
    {
        Task OnServerConnected(AsyncTcpSocketClient client);
        Task OnServerDataReceived(AsyncTcpSocketClient client, byte[] data, int offset, int count);
        Task OnServerDisconnected(AsyncTcpSocketClient client);
    }
}
