using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using TcpTapClientSocketLib;
using System.Threading;


namespace TcpDummyClientsLib
{
    public class ModuleSimpleEcho
    {
        protected Int64 IsStart = (int)Status.STOP;
        
        List<AsyncTcpSocketClient> DummyList = new List<AsyncTcpSocketClient>();
                

        // 최대 스레드 수만큼 나누어서 반복 작업을 시키자(그래야 스레드 다 활용할테니
        public async Task<string> Start(TestSimpleEchoConfig testConfig)
        {
            DummyList.Clear();

            var workList = new List<Task<string>>();
            var remoteEP = new IPEndPoint(IPAddress.Parse(testConfig.RemoteIP), testConfig.RemotePort);

            IsStart = (int)Status.PAUSE;

            for (int i = 0; i < testConfig.DummyCount; ++i)
            {
                var config = new AsyncTcpSocketClientConfiguration();
                config.FrameBuilder = new HeadBodyFrameBuilderBuilder();

                var msgDisp = new MessageDispatcher.EchoDispatcher();
                msgDisp.LogFunc = testConfig.LogFunc;

                var client = new AsyncTcpSocketClient(remoteEP, msgDisp, config);
                client.Connect().Wait();
                client.IsEnableEchoSend = true;

                workList.Add(Echo(client, testConfig.RepeatCount, testConfig.RepeatTime));

                DummyList.Add(client);
            }

            IsStart = (int)Status.RUN;

            await Task.WhenAll(workList.ToArray());

            var result = DummyResult(workList);
            return result;
        }

        async Task<string> Echo(AsyncTcpSocketClient client, int repeatCount, DateTime repeatTime)
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

                    if (client.IsEnableEchoSend)
                    {
                        client.IsEnableEchoSend = false;

                        var data = MakePacket();
                        await client.SendAsync(data);

                        ++workingCount;
                    }
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

            foreach (var ret in workList)
            {
                if (ret.Result.IndexOf("없음", 0) == 0)
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

        public async Task<string> End()
        {
            for (int i = 0; i < DummyList.Count; ++i)
            {
                await DummyList[i].Close();
            }

            return "접속 종료 OK";
        }
        
        protected enum Status
        {
            STOP = 0,
            PAUSE = 1,
            RUN = 2,
        }


        public Random RandDataSize = new Random();

        public byte[] MakePacket()
        {
            var length = RandDataSize.Next(32, 512);
            var text = Utils.RandomString(length);


            Int16 packetId = 241;
            var textLen = (Int16)Encoding.Unicode.GetBytes(text).Length;
            var bodyLen = (Int16)(textLen + 2);

            var sendData = new byte[4 + 2 + textLen];
            Buffer.BlockCopy(BitConverter.GetBytes(packetId), 0, sendData, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bodyLen), 0, sendData, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(textLen), 0, sendData, 4, 2);
            Buffer.BlockCopy(Encoding.Unicode.GetBytes(text), 0, sendData, 6, textLen);

            return sendData;
        }
    }


}