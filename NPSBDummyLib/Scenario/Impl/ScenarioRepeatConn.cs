using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ScenarioRepeatConn : ScenarioBase
    {
        public override string GetScenarioName()
        {
            return "RepeatConnect";
        }

        public override async Task<(bool, string)> TaskAsync(Dummy dummy, TestConfig config)
        {
            config.ScenarioName = GetScenarioName();
            var onlyConnect = ActionBase.MakeActionFactory(TestCase.ACTION_ONLY_CONNECT, config);
            var onlyDisConnect = ActionBase.MakeActionFactory(TestCase.ACTION_ONLY_DISCONNECT, config);

            var repeatCount = 0;
            var testStartTime = DateTime.Now;
            (bool, string) taskResult;

            while (true)
            {
                taskResult = await onlyConnect.Run(dummy);
                if (taskResult.Item1 == false)
                {
                    // 실패 통보하면서 더미 실행 중지
                    return (false, taskResult.Item2);
                }


                taskResult = await onlyDisConnect.Run(dummy);
                if (taskResult.Item1 == false)
                {
                    // 실패 통보하면서 더미 실행 중지
                    return (false, taskResult.Item2);
                }

                ++repeatCount;

                // 테스트 조건 검사
                if (repeatCount > config.ActionRepeatCount)
                {
                    break;
                }

                var elapsedTime = DateTime.Now - testStartTime;
                if (elapsedTime.TotalMilliseconds > config.LimitActionTime)
                {
                    Utils.MakeResult(dummy.Index, false, "타임 아웃");
                }
            }

            return Utils.MakeResult(dummy.Index, true, "Success");
        }
    }
}
