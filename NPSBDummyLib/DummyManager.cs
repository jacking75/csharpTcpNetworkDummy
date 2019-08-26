using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public partial class DummyManager
    {
        List<Dummy> DummyList = new List<Dummy>();

        TestConfig Config = new TestConfig();

        TestResultManager TestResultMgr = new TestResultManager();

        static bool InProgress;

        static DummyInfo DummyInfo;

        public Action<string> LogFunc; //[진행중] [완료] [실패]


        static Int64 CurrentConnectingCount = 0;
        static public Int64 ConnectedDummyCount()
        {
            return CurrentConnectingCount;
        }
        static public Int64 DummyConnected()
        {
            return Interlocked.Increment(ref CurrentConnectingCount);
        }
        static public Int64 DummyDisConnected()
        {
            return Interlocked.Decrement(ref CurrentConnectingCount);
        }

        public static DummyInfo GetDummyInfo
        {
            get
            {
                return DummyInfo;
            }
        }

        public static DummyInfo SetDummyInfo
        {
            set
            {
                DummyInfo = value;
            }
        }

        public Dummy GetDummy(int index)
        {
            if (index < 0 || index >= DummyList.Count)
            {
                return null;
            }

            return DummyList[index];
        }

        public void Init()
        {
            Clear();
            TestResultMgr.Clear();
        }

        public bool Prepare()
        {
            if (!IsInProgress())
            {
                CurrentConnectingCount = 0;
                DummyList.Clear();

                for (int i = 0; i < DummyManager.GetDummyInfo.DummyCount; ++i)
                {
                    var dummy = new Dummy();
                    dummy.Init(i);
                    DummyList.Add(dummy);   
                }
                InProgress = true;
            }

            return true;
        }

        public void SetConfigure(TestConfig config)
        {
            config.IsConditionFunc = IsInProgress;
            config.GetDummyFunc = GetDummy;
            Config = config;
        }

        public static void EndProgress()
        {
            InProgress = false;
        }

        public void EndTest()
        {
            InProgress = false;
            
            DummyList.Clear();
            Config.ActionCase = TestCase.NONE;
        }

        public void Clear()
        {
            EndTest();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                if (DummyList[i] == null)
                {
                    continue;
                }

                DummyList[i].DisConnect();
            }

            DummyList.Clear();
            CurrentConnectingCount = 0;
        }

        public static bool IsInProgress()
        {
            return InProgress;
        }

        public List<ReportData> GetTestResult(Int64 testIndex, TestConfig config)
        {
            return TestResultMgr.WriteTestResult(testIndex, config);
        }

        public string ToPacketStat()
        {
            var result = TestResultMgr.MakePacketStat();
            return result;
        }

        public TestCase CurrentTestCase()
        {
            return Config.ActionCase;
        }

        public bool AddTaskAction(int dummyIndex, TestCase testCase, TestConfig config)
        {
            var dummy = GetDummy(dummyIndex);
            if (dummy == null)
            {
                return false;
            }

            dummy.TaskMgr.AddTask(testCase, config);
            return true;
        }

        public async Task TaskStartAndWaitUntilTheEnd(TestConfig config)
        {
            var testUniqueId = config.TestUniqueId;
            var testResults = new List<Task>();
            var startTime = DateTime.Now;

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[i];

                // 더미간에는 비동기적으로 액션 처리
                testResults.Add(Task.Run(async () => {
                    await dummy.TaskMgr.RunAsync();
                }));
            }

            await Task.WhenAll(testResults.ToArray());

            TestResultMgr.AddTestResult(testUniqueId, config.ActionCase, DummyList, startTime);
        }


        // Host 프로그램에 메시지를 보낼 큐 혹은 델리게이트. 에러, 로그, 결과를 보냄
        // Host 프로그램에서 메시지를 받을 큐 혹은 델리게이트. 중단 메시지를 받음

        //System.Threading.Interlocked.Increment(ref ConnectedCount);
        //System.Threading.Interlocked.Decrement(ref ConnectedCount);
        //System.Threading.Interlocked.Read(ref ConnectedCount);
    }
}
