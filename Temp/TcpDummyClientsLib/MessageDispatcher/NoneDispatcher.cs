using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TcpTapClientSocketLib;

namespace TcpDummyClientsLib.MessageDispatcher
{
    class NoneDispatcher : IAsyncTcpSocketClientMessageDispatcher
    {
        public async Task OnServerConnected(AsyncTcpSocketClient client)
        {
            //Console.WriteLine(string.Format("TCP server {0} has connected.", client.RemoteEndPoint));
            await Task.CompletedTask;
        }

        public async Task OnServerDataReceived(AsyncTcpSocketClient client, byte[] data, int offset, int count)
        {
            //
            await Task.CompletedTask;
        }

        public async Task OnServerDisconnected(AsyncTcpSocketClient client)
        {
            //Console.WriteLine(string.Format("TCP server {0} has disconnected.", client.RemoteEndPoint));
            await Task.CompletedTask;
        }
    }
}
