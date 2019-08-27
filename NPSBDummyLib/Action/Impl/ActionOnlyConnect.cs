using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    internal class ActionOnlyConnect : ActionBase
    {
        public ActionOnlyConnect(TestConfig config) :
            base(TestCase.ACTION_ONLY_CONNECT, config)
        {
        }

        public override string GetActionName()
        {
            return "OnlyConnect";
        }

        protected override async Task<(bool, string)> TaskAsync(Dummy dummy)
        {

            var result = await dummy.ConnectAsyncAndReTry(DummyManager.GetDummyInfo.RmoteIP, DummyManager.GetDummyInfo.RemotePort);
            if (result.Result == false)
            {
                return Utils.MakeResult(dummy.Index, false, result.Error, this);
            }
            else
            {
                dummy.Connected();
            }

            dummy.SetSuccess(true);
            return Utils.MakeResult(dummy.Index, true, "");
        }
    }
}
