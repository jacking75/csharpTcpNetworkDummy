using CSBaseLib;
using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    internal class ActionOnlyDisconnect : ActionBase
    {
        private Int64 CurrentCount;

        public ActionOnlyDisconnect(TestConfig config) :
            base(TestCase.ACTION_ONLY_DISCONNECT, config)
        {
        }

        public override string GetActionName()
        {
            return "OnlyDisconnect";
        }

        protected override async Task<(bool, string)> TaskAsync(Dummy dummy)
        {
            CurrentCount = await Task.Run<Int64>(() => {
                var result = dummy.DisConnect();
                return result;

            });

            dummy.SetSuccess(true);
            return Utils.MakeResult(dummy.Index, true, "");
        }
    }
}
