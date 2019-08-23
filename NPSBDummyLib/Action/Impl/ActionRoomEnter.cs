using CSBaseLib;
using MessagePack;
using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ActionRoomEnter : ActionBase
    {
        public int RoomNumber { get; private set; }
        
        public ActionRoomEnter(DummyManager dummyMananger, TestConfig config) :
            base(TestCase.ACTION_ROOM_ENTER, dummyMananger, config)
        {
            RegistRecvFunc(PACKETID.RES_ROOM_ENTER, CheckResRoomEnter, true);
        }

        public override string GetActionName()
        {
            return "RoomEnter";
        }

        protected override async Task<(int, bool, string)> TaskAsync(Dummy dummy, TestConfig config)
        {
            var clientSocket = dummy.ClientSocket;
            try
            {
                // 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                //Utils.Logger.Debug($"Echo-Send. ClientIndex: {dummy.Index}");
                RoomNumber = config.RoomNumber;
                var packet = new PKTReqRoomEnter()
                {
                    RoomNumber = config.RoomNumber,
                };

                var sendData = PacketToBytes.Make(PACKETID.REQ_ROOM_ENTER, packet);
                var sendError = await clientSocket.SendAsync(sendData.Length, sendData);
                if (sendError != "")
                {
                    return End(dummy, false, sendError);
                }

                dummy.IncreasePacket(PACKETID.REQ_ROOM_ENTER);

                return await RecvProc(dummy);
            }
            catch (Exception ex)
            {
                return (dummy.Index, false, ex.ToString());
            }
        }


        private (int, bool, string) CheckResRoomEnter(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTResRoomEnter>(packetBuffer);
            if ((ERROR_CODE)body.Result != ERROR_CODE.NONE)
            {
                return (dummy.Index, false, $"결과값 틀림({body.Result})");
            }

            dummy.RoomNumber = RoomNumber;

            return (dummy.Index, true, "");
        }
    }
}
