using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    internal class ActionConnect : ActionBase
    {
        public ActionConnect(DummyManager dummyManange, TestConfig config) :
            base(TestCase.ACTION_CONNECT, dummyManange, config)
        {
        }

        public override string GetActionName()
        {
            return "Connect";
        }

        protected override async Task<(int, bool, string)> TaskAsync(Dummy dummy, TestConfig config)
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


        public static async Task<(int, bool, string)> RepeatConnectAsync(Dummy dummy, TestConfig config, Func<bool> isContinue)
        {
            var expectConnectCount = config.RepeatConnectCount;
            var expectDateTime = DateTime.Now.AddSeconds(config.RepeatConnectDateTimeSec);

            while (isContinue())
            {
                var result = await dummy.ConnectAsyncAndReTry(DummyManager.GetDummyInfo.RmoteIP, DummyManager.GetDummyInfo.RemotePort);
                if (result.Result == false)
                {
                    return (dummy.Index, false, result.Error);
                }


                dummy.Connected();
                dummy.DisConnect();

                if (expectConnectCount > 0 && expectConnectCount == dummy.ConnectCount)
                {
                    dummy.SetSuccess(true);
                    break;
                }
                else if (expectConnectCount == 0 && expectDateTime <= DateTime.Now)
                {
                    dummy.SetSuccess(true);
                    break;
                }
            }

            return (dummy.Index, true, "");
        }
    }
}
