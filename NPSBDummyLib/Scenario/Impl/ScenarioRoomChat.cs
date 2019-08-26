using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ScenarioRoomChat : ScenarioBase
    {
        public override async Task<(bool, string)> TaskAsync(Dummy dummy, TestConfig config)
        {
            var connect = MakeActionFactory(TestCase.ACTION_CONNECT, config);
            var login = MakeActionFactory(TestCase.ACTION_LOGIN, config);
            var roomEnter = MakeActionFactory(TestCase.ACTION_ROOM_ENTER, config);
            var roomLeave = MakeActionFactory(TestCase.ACTION_ROOM_LEAVE, config);
            var roomChat = MakeActionFactory(TestCase.ACTION_ROOM_CHAT, config);
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
                    return (false, taskResult.Item2);
                }

                taskResult = await login.Run(dummy);
                if (taskResult.Item1 == false)
                {
                    // 실패 통보하면서 더미 실행 중지
                    return (false, taskResult.Item2);
                }
                
                taskResult = await roomEnter.Run(dummy);
                if (taskResult.Item1 == false)
                {
                    // 실패 통보하면서 더미 실행 중지
                    return (false, taskResult.Item2);
                }

                taskResult = await roomChat.Run(dummy);
                if (taskResult.Item1 == false)
                {
                    // 실패 통보하면서 더미 실행 중지
                    return (false, taskResult.Item2);
                }

                taskResult = await roomLeave.Run(dummy);
                if (taskResult.Item1 == false)
                {
                    // 실패 통보하면서 더미 실행 중지
                    return (false, taskResult.Item2);
                }

                taskResult = await disConnect.Run(dummy);
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

