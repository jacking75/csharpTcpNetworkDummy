﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using TcpTapClientSocketLib;

namespace TcpDummyClientsLib
{
    public class TestConnectOnly
    {
        Int64 ConnectedCount = 0;
        List<AsyncTcpSocketClient> DummyList = new List<AsyncTcpSocketClient>();

        public async Task<string> Start(int dummyCount, string ip, UInt16 port)
        {
            ConnectedCount = 0;
            DummyList.Clear();

            var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

            try
            {
                for (int i = 0; i < dummyCount; ++i)
                {
                    var config = new AsyncTcpSocketClientConfiguration();
                    config.FrameBuilder = new FixedLengthFrameBuilder(8);

                    DummyList.Add(new AsyncTcpSocketClient(remoteEP, new SimpleMessageDispatcher(), config));
                }

                for (int i = 0; i < dummyCount; ++i)
                {
                    await DummyList[i].Connect();

                    System.Threading.Interlocked.Increment(ref ConnectedCount);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return $"접속 완료 OK: {ConnectedCount}";
        }

        public async Task<string> End()
        {
            for (int i = 0; i < DummyList.Count; ++i)
            {
                await DummyList[i].Close();
                                        
                System.Threading.Interlocked.Decrement(ref ConnectedCount);
            }

            return $"접속 종료 OK. 현재 접속 중인 수: {ConnectedCount}";
        }

        public Int64 CurrentConnectedCount()
        {
            return System.Threading.Interlocked.Read(ref ConnectedCount);
        }

        class SimpleMessageDispatcher : IAsyncTcpSocketClientMessageDispatcher
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
}