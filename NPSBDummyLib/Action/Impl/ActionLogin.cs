using CSBaseLib;
using MessagePack;
using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ActionLogin : ActionBase
    {
        public ActionLogin(TestConfig config) : 
            base(TestCase.ACTION_LOGIN, config)
        {
            RegistRecvFunc(PACKETID.RES_LOGIN, CheckResLogin, true);
        }

        public override string GetActionName()
        {
            return "Login";
        }

        protected override async Task<(bool, string)> TaskAsync(Dummy dummy)
        {
            var clientSocket = dummy.ClientSocket;
            try
            {
                // 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                //Utils.Logger.Debug($"Echo-Send. ClientIndex: {dummy.Index}");

                var packet = new PKTReqLogin()
                {
                    UserID = dummy.GetUserID(),
                    AuthToken = "abcde",
                };

                var sendData = PacketToBytes.Make(PACKETID.REQ_LOGIN, packet);
                var sendError = await clientSocket.SendAsync(sendData.Length, sendData);
                if (sendError != "")
                {
                    return End(dummy, false, sendError);
                }

                dummy.IncreasePacket(PACKETID.REQ_LOGIN);

                return await RecvProc(dummy);
            }
            catch (Exception ex)
            {
                return Utils.MakeResult(dummy.Index, false, ex.ToString());
            }
        }


        private (bool, string) CheckResLogin(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTResLogin>(packetBuffer);
            if ((ERROR_CODE)body.Result != ERROR_CODE.NONE)
            {
                return Utils.MakeResult(dummy.Index, false, $"결과값 틀림({body.Result})");
            }

            return Utils.MakeResult(dummy.Index, true, "");
        }
    }
}
