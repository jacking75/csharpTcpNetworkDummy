using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TcpTapClientSocketLib;

namespace TcpDummyClientsLib.MessageDispatcher
{
    class EchoDispatcher : IAsyncTcpSocketClientMessageDispatcher
    {
        public Action<string> LogFunc;


        public async Task OnServerConnected(AsyncTcpSocketClient client)
        {
            LogFunc($"TCP server {client.RemoteEndPoint} has connected.");
            await Task.CompletedTask;
        }

        public async Task OnServerDataReceived(AsyncTcpSocketClient client, byte[] data, int offset, int count)
        {
            client.IsEnableEchoSend = true;
            await Task.CompletedTask;
        }

        public async Task OnServerDisconnected(AsyncTcpSocketClient client)
        {
            LogFunc($"TCP server {client.RemoteEndPoint} has disconnected.");
            await Task.CompletedTask;
        }
    }
}
