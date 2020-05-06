using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public enum TestCase
    {
        NONE = 0,
        ONLY_CONNECT = 1,

        REPEAT_CONNECT = 2,
        ECHO = 3,
        ECHO_CONNECT_DISCONNECT = 4,
        ECHO_CONNECT_DISCONNECT_RANDOM = 5,
        ECHO_CONNECT_DISCONNECT_SERVER = 6,


        ACTION_CONNECT = 100,
        ACTION_DISCONNECT = 101,
        ACTION_LOGIN = 102,
        ACTION_ROOM_ENTER = 103,
        ACTION_ROOM_LEAVE = 104,
        ACTION_ROOM_CHAT = 105,
        ACTION_ONLY_CONNECT = 106,
        ACTION_ONLY_DISCONNECT = 107,
    }


    public partial class DummyManager
    {
        public async Task RunAction(Int64 testUniqueId, TestCase testType, TestConfig config)
        {
            var testResults = new List<Task<(bool, string)>>();
            var actionList = new List<ActionBase>();
            var startTime = DateTime.Now;

            testResults.Capacity = DummyList.Count;
            actionList.Capacity = DummyList.Count;

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[i];

                var action = ActionBase.MakeActionFactory(testType, config);
                action.Before(dummy);
                actionList.Add(action);

                if (config.DummyIntervalTime > 0)
                {
                    await Task.Delay(config.DummyIntervalTime);
                }

                testResults.Add(Task<(bool, string)>.Run(() => {
                    return action.Run(dummy);
                }));
            }

            await Task.WhenAll(testResults.ToArray());

            TestResultMgr.AddTestResult(testUniqueId, testType, DummyList, startTime, testResults);

            foreach (var action in actionList)
            {
                action.After();
            }
        }

        //TODO 아래 로그인 테스트처럼 변경하자
        public async Task TestConnectOnlyAsync(Int64 testUniqueId)
        {
            var startTime = DateTime.Now;
            var testResults = new List<Task<(bool, string)>>();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[i];
                testResults.Add(Task<(bool, string)>.Run(() => ActionNetConnect.ConnectOnlyAsync(dummy, Config, IsInProgress)));
            }

            await Task.WhenAll(testResults.ToArray());
                                    
            TestResultMgr.AddTestResult(testUniqueId, Config.ActionCase, DummyList, startTime);
        }

        //TODO 아래 로그인 테스트처럼 변경하자
        public async Task TestRepeatConnectAsync(Int64 testUniqueId)
        {
            var startTime = DateTime.Now;
            var testResults = new List<Task<(bool, string)>>();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[i];
                testResults.Add(Task<(bool, string)>.Run(() => ActionNetConnect.RepeatConnectAsync(dummy, Config, IsInProgress)));
            }

            await Task.WhenAll(testResults.ToArray());

            TestResultMgr.AddTestResult(testUniqueId, Config.ActionCase, DummyList, startTime);
        }

        //TODO 아래 로그인 테스트처럼 변경하자
        public async Task TestRepeatEchoAsync(Int64 testUniqueId, EchoCondition echoCondi)
        {
            var startTime = DateTime.Now;
            var testResults = new List<Task<(bool, string)>>();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[i];
                testResults.Add(Task<(bool, string)>.Run(async () => {
                    var echoAction = new ActionEcho();
                        return await echoAction.EchoAsync(dummy, echoCondi);
                    }));
            }

            await Task.WhenAll(testResults.ToArray());

            TestResultMgr.AddTestResult(testUniqueId, Config.ActionCase, DummyList, startTime);
        }

        public async Task RunTestScenario(Int64 testUniqueId, TestConfig config, Func<Dummy, DateTime, Task<(bool, string)>> func)
        {
            var startTime = DateTime.Now;

            SetConfigure(config);
            var testResults = new List<Task<(bool, string)>>();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[i];
                testResults.Add(Task<(bool, string)>.Run(async () =>
                {
                    return await func(dummy, startTime);
                }));
            }

            await Task.WhenAll(testResults.ToArray());

            TestResultMgr.AddTestResult(testUniqueId, Config.ActionCase, DummyList, startTime, testResults);
        }


        async Task RunTestScenario(Int64 testUniqueId, TestConfig config, List<Dummy> dummyList, Action<List<Task<(bool, string)>>, Dummy, Int64, TestConfig> scenarioTask)
        {
            var startTime = DateTime.Now;
            var testResults = new List<Task<(bool, string)>>();
            SetConfigure(config);

            for (int i = 0; i < dummyList.Count; ++i)
            {
                var dummy = dummyList[i];
                scenarioTask(testResults, dummy, testUniqueId, config);
            }

            await Task.WhenAll(testResults.ToArray());

            TestResultMgr.AddTestResult(testUniqueId, Config.ActionCase, DummyList, startTime, testResults);
        }

        public async Task TestLoginAsync(Int64 testUniqueId, TestConfig config)
        {
            await RunTestScenario(testUniqueId, config, DummyList, AddDummyScenarioLogin);            
        }

        void AddDummyScenarioLogin(List<Task<(bool, string)>> results, Dummy dummy, Int64 testUniqueId, TestConfig config)
        {
            results.Add(Task<(bool, string)>.Run(async () => {

                var scenario = new ScenarioLogin();
                return await scenario.TaskAsync(dummy, config);
            }));
        }

        public async Task TestRepeatConnAsync(Int64 testUniqueId, TestConfig config)
        {
            await RunTestScenario(testUniqueId, config, DummyList, AddDummyScenarioRepeatConn);
        }

        void AddDummyScenarioRepeatConn(List<Task<(bool, string)>> results, Dummy dummy, Int64 testUniqueId, TestConfig config)
        {
            results.Add(Task<(bool, string)>.Run(async () => {

                var scenario = new ScenarioRepeatConn();
                return await scenario.TaskAsync(dummy, config);
            }));
        }

        public async Task TestRoomChatAsync(Int64 testUniqueId, TestConfig config)
        {
            await RunTestScenario(testUniqueId, config, DummyList, AddDummyScenarioRoomChat);
        }

        void AddDummyScenarioRoomChat(List<Task<(bool, string)>> results, Dummy dummy, Int64 testUniqueId, TestConfig config)
        {
            results.Add(Task<(bool, string)>.Run(async () => {

                var scenario = new ScenarioRoomChat();
                return await scenario.TaskAsync(dummy, config);
            }));
        }
    }
}
