using CSBaseLib;
using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    internal class ActionDisconnect : ActionBase
    {
        private Int64 CurrentCount;

        public ActionDisconnect(TestConfig config) :
            base(TestCase.ACTION_DISCONNECT, config)
        {
            RegistRecvFunc(PACKETID.CS_END, CheckEnd, true);
        }

        public override string GetActionName()
        {
            return "Disconnect";
        }

        protected override async Task<(bool, string)> TaskAsync(Dummy dummy)
        {
            CurrentCount = await Task.Run<Int64>(() => {
                var result = dummy.DisConnect();
                return result;

            });

            return await RecvProc(dummy);
        }


        private (bool, string) CheckEnd(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            return Utils.MakeResult(dummy.Index, true, $"Connected dummy count : {CurrentCount}");
        }
    }
}
