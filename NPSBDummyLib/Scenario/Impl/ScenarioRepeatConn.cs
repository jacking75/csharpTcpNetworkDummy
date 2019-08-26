using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ScenarioRepeatConn : ScenarioBase
    {
        public override async Task<(bool, string)> TaskAsync(Dummy dummy, TestConfig config)
        {
            var connect = MakeActionFactory(TestCase.ACTION_CONNECT, config);
            var disConnect = MakeActionFactory(TestCase.ACTION_DISCONNECT, config);

            var repeatCount = 0;
            var testStartTime = DateTime.Now;
            (bool, string) taskResult;

            while (true)
            {
                taskResult = await connect.Run(dummy);
                if (taskResult.Item1 == false)
                {
                    // 실패 통보하면서 더미 실행 중지
                    return (false, "fail Connect");
                }


                taskResult = await disConnect.Run(dummy);
                if (taskResult.Item1 == false)
                {
                    // 실패 통보하면서 더미 실행 중지
                    return (false, "fail DisConnect");
                }

                ++repeatCount;

                // 테스트 조건 검사
                if (repeatCount > config.ActionRepeatCount)
                {
                    break;
                }

            }

            return Utils.MakeResult(dummy.Index, true, "Success");
        }
    }
}
