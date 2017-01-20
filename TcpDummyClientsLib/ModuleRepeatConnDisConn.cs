using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

using TcpTapClientSocketLib;

namespace TcpDummyClientsLib
{
    class ModuleRepeatConnDisConn
    {
        Int64 IsStart = 0;

        List<AsyncTcpSocketClient> DummyList = new List<AsyncTcpSocketClient>();


        // 최대 스레드 수만큼 나누어서 반복 작업을 시키자(그래야 스레드 다 활용할테니
        public string Start(int dummyCount, int repeatCount, string ip, UInt16 port)
        {
            var maxThreadCount = Utils.MinMaxThreadCount().Item2;
            var 몫과나머지 = Utils.나누기_몫과나머지(maxThreadCount, dummyCount);
            
            DummyList.Clear();

            var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

            try
            {
                for (int i = 0; i < dummyCount; ++i)
                {
                    var config = new AsyncTcpSocketClientConfiguration();
                    config.FrameBuilder = new FixedLengthFrameBuilder(8);

                    DummyList.Add(new AsyncTcpSocketClient(remoteEP, new MessageDispatcher.NoneDispatcher(), config));
                }

                var workList = new List<Task<string>>();

                if (몫과나머지.Item1 == maxThreadCount)
                {
                    for (int i = 0; i < maxThreadCount; ++i)
                    {
                        for (int j = 0; j < 몫과나머지.Item1; ++j)
                        {
                            //workList.Add(DummyList[i].Connect() );
                        }
                    }

                }

                for (int i = 0; i < 몫과나머지.Item2; ++i)
                {
                    //await DummyList[i].Connect();
                }

                Task.WaitAll(workList.ToArray());

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return $"접속 완료 OK: {dummyCount}";
        }

        
        class RepeatConnDisConn
        {
            Int64 IsStart = 0;
            List<AsyncTcpSocketClient> DummyList = new List<AsyncTcpSocketClient>();
            Dictionary<AsyncTcpSocketClient, int> SocketRepeatCount = new Dictionary<AsyncTcpSocketClient, int>();

            public async Task<string> Start(int dummyCount, int wantRepeatCount, IPEndPoint remoteEP)
            {
                int tryRepeatCount = 0;
                try
                {
                    for (int i = 0; i < dummyCount; ++i)
                    {
                        var config = new AsyncTcpSocketClientConfiguration();
                        config.FrameBuilder = new FixedLengthFrameBuilder(8);

                        var socketClient = new AsyncTcpSocketClient(remoteEP, new MessageDispatcher.NoneDispatcher(), config);
                        DummyList.Add(socketClient);

                        SocketRepeatCount.Add(socketClient, 0);
                    }

                    for (int i = 0; i < dummyCount; ++i)
                    {
                        // 소켓의 반복 횟수가 끝났으면 더 안한다.                        
                        //++tryRepeatCount;
                        //if(tryRepeatCount == wantRepeatCount) { break; }
                        await DummyList[i].Connect();
                        await DummyList[i].Close();

                        // //SocketRepeatCount을 뒤져서 값을 올린다.
                    }

                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                return $"접속 완료 OK: {dummyCount}";
            }
        }

        
    }
}
