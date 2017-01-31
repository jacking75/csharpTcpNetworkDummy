using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


using TcpTapClientSocketLib;

namespace TcpDummyClient
{
    class TestSendReceive
    {
        AsyncTcpSocketClient Client;

        public Action<string> LogFunc;
        
        public string Connect(string ip, UInt16 port)
        {
            try
            {
                var config = new AsyncTcpSocketClientConfiguration();
                config.FrameBuilder = new HeadBodyFrameBuilderBuilder();

                var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
                Client = new AsyncTcpSocketClient(remoteEP, new SimpleMessageDispatcher(), config);
                Client.Connect().Wait();

                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void SendData(string text)
        {
            var data = MakePacket(text);
            Client.SendAsync(data).Wait();
        }

        public byte[] MakePacket(string text)
        {
            Int16 packetId = 11;
            var textLen = (Int16)Encoding.Unicode.GetBytes(text).Length;

            var sendData = new byte[4 + textLen];
            Buffer.BlockCopy(sendData, 0, BitConverter.GetBytes(packetId), 0, 2);
            Buffer.BlockCopy(sendData, 2, BitConverter.GetBytes(textLen), 0, 2);
            Buffer.BlockCopy(sendData, 4, Encoding.Unicode.GetBytes(text), 0, textLen);

            return sendData;
        }


        class SimpleMessageDispatcher : IAsyncTcpSocketClientMessageDispatcher
        {
            public Action<string> LogFunc;


            public async Task OnServerConnected(AsyncTcpSocketClient client)
            {
                LogFunc($"TCP server {client.RemoteEndPoint} has connected.");
                await Task.CompletedTask;
            }

            public async Task OnServerDataReceived(AsyncTcpSocketClient client, byte[] data, int offset, int count)
            {
                //LogFunc($"TCP server {client.RemoteEndPoint} has connected.");
                await Task.CompletedTask;
            }

            public async Task OnServerDisconnected(AsyncTcpSocketClient client)
            {
                LogFunc($"TCP server {client.RemoteEndPoint} has disconnected.");
                await Task.CompletedTask;
            }
        }
    }
}
