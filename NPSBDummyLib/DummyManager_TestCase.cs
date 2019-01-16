using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public partial class DummyManager
    {
        public async Task<(bool, string)> TestConnectOnlyAsync(int dummyIndex)
        {            
            var testResults = new List<Task<(bool, string)>>();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[dummyIndex];
                testResults.Add(Task<(bool, string)>.Run(() => NetActionConnect.ConnectOnlyAsync(dummy, Config, IsInProgress)));
            }

            await Task.WhenAll(testResults.ToArray());

            return (false, "error");
        }

        public async Task<(bool, string)> TestRepeatConnectAsync(int dummyIndex)
        {
            var testResults = new List<Task<(bool, string)>>();

            for (int i = 0; i < DummyList.Count; ++i)
            {
                var dummy = DummyList[dummyIndex];
                testResults.Add(Task<(bool, string)>.Run(() => NetActionConnect.RepeatConnectAsync(dummy, Config, IsInProgress)));
            }

            await Task.WhenAll(testResults.ToArray());

            return (false, "error");
        }
    }
}
