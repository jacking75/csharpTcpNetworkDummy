using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    internal class ActionDisconnect : ActionBase
    {
        public ActionDisconnect(TestConfig config) :
            base(TestCase.ACTION_DISCONNECT, config)
        {
        }

        public override string GetActionName()
        {
            return "Disconnect";
        }

        protected override async Task<(int, bool, string)> TaskAsync(Dummy dummy)
        {
            var currentCount = await Task.Run<Int64>(() => {
                var result = dummy.DisConnect();
                return result;

            });

            DummyManager.EndProgress();
            return (dummy.Index, true, $"Connected dummy count : {currentCount}");

        }
    }
}
