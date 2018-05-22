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
        

        public bool IsStart { get; private set; }



        //public Action<string> MsgFunc; //[진행중] [완료] [실패]

        public Action<string> LogFunc; //[진행중] [완료] [실패]

                
        public bool Prepare(TestConfig config)
        {
            Config = config;
            return false;
        }

        public void EndTest()
        {
            IsStart = false;

            //TODO:모든 더미가 작업이 다 완료 되었다면 더미를 다 지운다.
            DummyList.Clear();
        }


        public async Task<(bool, string)> TestRepeatConnectAsync(int dummyIndex)
        {
            var testResults = new List<Task<(bool, string)>>();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                testResults.Add(Task<(bool, string)>.Run(() => RepeatConnectAsync(i)));
            }

            await Task.WhenAll(testResults.ToArray());

            return (false, "error");
        }

            // Host 프로그램에 메시지를 보낼 큐 혹은 델리게이트. 에러, 로그, 결과를 보냄
            // Host 프로그램에서 메시지를 받을 큐 혹은 델리게이트. 중단 메시지를 받음

            //System.Threading.Interlocked.Increment(ref ConnectedCount);
            //System.Threading.Interlocked.Decrement(ref ConnectedCount);
            //System.Threading.Interlocked.Read(ref ConnectedCount);
    }
}
