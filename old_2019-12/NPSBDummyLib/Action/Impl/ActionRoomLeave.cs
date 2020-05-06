using CSBaseLib;
using MessagePack;
using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ActionRoomLeave : ActionBase
    {
        public ActionRoomLeave(TestConfig config) :
            base(TestCase.ACTION_ROOM_LEAVE, config)
        {
            RegistRecvFunc(PACKETID.RES_ROOM_LEAVE, CheckResRoomLeave, true);    
        }

        public override string GetActionName()
        {
            return "RoomLeave";
        }

        protected override async Task<(bool, string)> TaskAsync(Dummy dummy)
        {
            var clientSocket = dummy.ClientSocket;
            try
            {
                // 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                //Utils.Logger.Debug($"Echo-Send. ClientIndex: {dummy.Index}");
                var packet = new PKTReqRoomLeave();
                var sendData = PacketToBytes.Make(PACKETID.REQ_ROOM_LEAVE, packet);
                var sendError = await clientSocket.SendAsync(sendData.Length, sendData);
                if (sendError != "")
                {
                    return End(dummy, false, sendError);
                }

                dummy.IncreasePacket(PACKETID.REQ_ROOM_LEAVE);

                return await RecvProc(dummy);
            }
            catch (Exception ex)
            {
                return Utils.MakeResult(dummy.Index, false, ex.ToString(), this);
            }
        }


        private (bool, string) CheckResRoomLeave(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTResRoomLeave>(packetBuffer);
            if ((ERROR_CODE)body.Result != ERROR_CODE.NONE)
            {
                return Utils.MakeResult(dummy.Index, false, $"결과값 틀림({body.Result})", this);
            }

            dummy.RoomNumber = 0;

            return Utils.MakeResult(dummy.Index, true, "");
        }
    }
}
