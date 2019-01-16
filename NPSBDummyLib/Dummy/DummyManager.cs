using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Dummy
{
    public partial class DummyManager
    {
        List<Dummy> DummyList = new List<Dummy>();

        TestConfig Config = new TestConfig();

        bool InProgress;



        //public Action<string> MsgFunc; //[진행중] [완료] [실패]

        public Action<string> LogFunc; //[진행중] [완료] [실패]

                
        public bool Prepare(TestConfig config)
        {
            Config = config;
            return false;
        }
        
        public void StartTest()
        {
            InProgress = true;
        }

        public void EndTest()
        {
            InProgress = false;
            DummyList.Clear();
        }

        public bool IsInProgress()
        {
            return InProgress;
        }





            // Host 프로그램에 메시지를 보낼 큐 혹은 델리게이트. 에러, 로그, 결과를 보냄
            // Host 프로그램에서 메시지를 받을 큐 혹은 델리게이트. 중단 메시지를 받음

        //System.Threading.Interlocked.Increment(ref ConnectedCount);
        //System.Threading.Interlocked.Decrement(ref ConnectedCount);
        //System.Threading.Interlocked.Read(ref ConnectedCount);
    }
}
