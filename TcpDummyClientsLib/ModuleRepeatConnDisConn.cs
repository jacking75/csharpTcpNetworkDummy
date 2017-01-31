using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

using TcpTapClientSocketLib;
using System.Threading;

namespace TcpDummyClientsLib
{
    class ModuleRepeatConnDisConn
    {
        protected Int64 IsStart = (int)Status.STOP;

        List<AsyncTcpSocketClient> DummyList = new List<AsyncTcpSocketClient>();


        // 최대 스레드 수만큼 나누어서 반복 작업을 시키자(그래야 스레드 다 활용할테니
        public string Start(int dummyCount, int repeatCount, DateTime repeatTime, string ip, UInt16 port)
        {
            DummyList.Clear();

            var workList = new List<Task<string>>();
            var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

            IsStart = (int)Status.PAUSE;

            for (int i = 0; i < dummyCount; ++i)
            {
                var config = new AsyncTcpSocketClientConfiguration();
                config.FrameBuilder = new FixedLengthFrameBuilder(8);

                DummyList.Add(new AsyncTcpSocketClient(remoteEP, new MessageDispatcher.NoneDispatcher(), config));


                workList.Add(ReConnect(DummyList[i], repeatCount, repeatTime));
            }

            IsStart = (int)Status.RUN;

            Task.WaitAll(workList.ToArray());                        
            
            return DummyResult(workList);
        }

        async Task<string> ReConnect(AsyncTcpSocketClient client, int repeatCount, DateTime repeatTime)
        {
            try
            {
                int workingCount = 0;

                while (true)
                {
                    if (Interlocked.Read(ref IsStart) == (Int64)Status.STOP)
                    {
                        return "중단";
                    }

                    if (Interlocked.Read(ref IsStart) == (Int64)Status.PAUSE)
                    {
                        await Task.Delay(1);
                    }

                    if (repeatCount != 0 && repeatCount == workingCount)
                    {
                        break;
                    }

                    if (repeatTime >= DateTime.Now)
                    {
                        break;
                    }

                    await client.Connect();
                    await client.Close();

                    ++workingCount;
                }
            }
            catch (Exception ex)
            {
                return "에러:" + ex.Message;
            }

            return "완료";
        }

        string DummyResult(List<Task<string>> workList)
        {
            int failCount = 0, successCount = 0;

            foreach(var ret in workList)
            {
                if(ret.Result.IndexOf("없음", 0) == 0)
                {
                    ++failCount;
                }
                else
                {
                    ++successCount;
                }
            }

            return $"접속 완료 OK: 성공 수{successCount}, 실패 수{failCount}";
        }

        protected enum Status
        {
            STOP = 0,
            PAUSE = 1,
            RUN = 2,
        }      

        
    }
}
