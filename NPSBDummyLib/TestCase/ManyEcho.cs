using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.TestCase
{
    //TODO: 삭제 예정
    /*
    public class ManyEcho
    {
        Int64 ConnectedCount = 0;
        List<Dummy.Echo> DummyList = new List<Dummy.Echo>();

        public Action<string> MsgFunc;

        public string Prepare(int dummyCount)
        {
            for (int i = 0; i < dummyCount; ++i)
            {
                DummyList.Add(new Dummy.Echo());
            }

            return "";
        }

        public async Task<string> ProcessAsync(Dummy.EchoCondition cond)
        {
            ConnectedCount = 0;

            try
            {
                for (int i = 0; i < DummyList.Count; ++i)
                {
                    await Task.Run(async () => await DummyList[i].ProcessAsync(cond));

                    System.Threading.Interlocked.Increment(ref ConnectedCount);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return $"[Success] 접속 완료: {ConnectedCount}";
        }               
    }
    */
}
