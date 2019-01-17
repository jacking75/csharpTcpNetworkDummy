using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public partial class DummyManager
    {
        List<Dummy> DummyList = new List<Dummy>();

        TestConfig Config = new TestConfig();

        TestResultManager TestResultMgr = new TestResultManager();

        bool InProgress;
                
        public Action<string> LogFunc; //[진행중] [완료] [실패]


        static Int64 CurrentConnectingCount = 0;
        static public Int64 ConnectedDummyCount()
        {
            return CurrentConnectingCount;
        }
        static public void DummyConnected()
        {
            Interlocked.Increment(ref CurrentConnectingCount);
        }
        static public void DummyDisConnected()
        {
            Interlocked.Decrement(ref CurrentConnectingCount);
        }

        public bool Prepare(TestConfig config)
        {
            CurrentConnectingCount = 0;
            DummyList.Clear();

            Config = config;

            for(int i = 0; i < Config.DummyCount; ++i)
            {
                DummyList.Add(new Dummy());
            }
            InProgress = true;
            
            return false;
        }
                
        public void EndTest()
        {
            InProgress = false;

            Thread.Sleep(2000);

            for (int i = 0; i < Config.DummyCount; ++i)
            {
                if(DummyList[i] == null)
                {
                    continue;
                }

                DummyList[i].DisConnect();
            }
            DummyList.Clear();

            Config.ActionCase = TestCase.NONE;
        }

        public bool IsInProgress()
        {
            return InProgress;
        }

        public List<string> GetTestResult(Int64 testIndex )
        {
            return TestResultMgr.WriteTestResult(testIndex, Config);
        }

        public TestCase CurrentTestCase()
        {
            return Config.ActionCase;
        }





            // Host 프로그램에 메시지를 보낼 큐 혹은 델리게이트. 에러, 로그, 결과를 보냄
            // Host 프로그램에서 메시지를 받을 큐 혹은 델리게이트. 중단 메시지를 받음

        //System.Threading.Interlocked.Increment(ref ConnectedCount);
        //System.Threading.Interlocked.Decrement(ref ConnectedCount);
        //System.Threading.Interlocked.Read(ref ConnectedCount);
    }
}
