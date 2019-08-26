using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public abstract class ScenarioBase
    {
        List<ActionBase> ActionTaskList;
        protected List<(int, bool, string)> ResultList;


        public ScenarioBase()
        {
            ActionTaskList = new List<ActionBase>();
            ResultList = new List<(int, bool, string)>();
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

        public List<(int, bool, string)> GetResultList()
        {
            return ResultList;
        }

        public abstract Task TaskAsync(Dummy dummy, TestConfig config);
    }
}
