using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    internal class ActionConnect : ActionBase
    {
        public ActionConnect(TestConfig config) :
            base(TestCase.ACTION_CONNECT, config)
        {
        }

        public override string GetActionName()
        {
            return "Connect";
        }

        protected override async Task<(int, bool, string)> TaskAsync(Dummy dummy)
        {
            var result = await dummy.ConnectAsyncAndReTry(DummyManager.GetDummyInfo.RmoteIP, DummyManager.GetDummyInfo.RemotePort);
            if (result.Result == false)
            {
                return (dummy.Index, false, result.Error);
            }
            else
            {
                dummy.Connected();
                dummy.CreateRecvWorker();
            }

            dummy.SetSuccess(true);
            return (dummy.Index, true, "");
        }
    }
}
