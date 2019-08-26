using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public class TaskActionManager
    {
        List<ActionBase> ActionTaskList;
        List<(bool, string)> ResultList;
        Dummy Owner;

        public TaskActionManager(Dummy owner)
        {
            ActionTaskList = new List<ActionBase>();
            ResultList = new List<(bool, string)>();
            Owner = owner;
        }

        public ActionBase MakeActionFactory(TestCase testType, TestConfig config)
        {
            ActionBase action = null;
            switch (testType)
            {
                case TestCase.ACTION_CONNECT:
                    action = new ActionConnect(config);
                    break;

                case TestCase.ACTION_DISCONNECT:
                    action = new ActionDisconnect(config);
                    break;

                case TestCase.ACTION_LOGIN:
                    action = new ActionLogin(config);
                    break;

                case TestCase.ACTION_ROOM_ENTER:
                    action = new ActionRoomEnter(config);
                    break;

                case TestCase.ACTION_ROOM_LEAVE:
                    action = new ActionRoomLeave(config);
                    break;

                case TestCase.ACTION_ROOM_CHAT:
                    action = new ActionRoomChat(config);
                    break;

                default:
                    break;
            }

            return action;
        }

        public void AddTask(TestCase testType, TestConfig config)
        {
            ActionTaskList.Add(MakeActionFactory(testType, config));

        }

        public async Task RunAsync()
        {
            // 더미에 등록한 액션은 순차적으로 처리
            foreach (var action in ActionTaskList)
            {
                ResultList.Add(await action.Run(Owner));
            }
        }

        public List<(bool, string)> GetResultList()
        {
            return ResultList;
        }
    }
}
