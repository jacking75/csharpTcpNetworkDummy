using System;
using System.Collections.Generic;
using System.Text;
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
    }


    public partial class DummyManager
    {
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
    }
}
