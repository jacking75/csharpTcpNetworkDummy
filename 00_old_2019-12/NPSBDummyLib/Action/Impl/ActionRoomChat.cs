using CSBaseLib;
using MessagePack;
using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ActionRoomChat : ActionBase
    {
        public string ChatMessage { get; private set; }

        public ActionRoomChat(TestConfig config) :
            base(TestCase.ACTION_ROOM_CHAT, config)
        {
        }

        public override string GetActionName()
        {
            return "RoomChat";
        }

        protected override async Task<(bool, string)> TaskAsync(Dummy dummy)
        {
            var clientSocket = dummy.ClientSocket;
            try
            {
                // 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                //Utils.Logger.Debug($"Echo-Send. ClientIndex: {dummy.Index}");

                ChatMessage = string.Copy(TestConfig.ChatMessage);
                var packet = new PKTReqRoomChat()
                {
                    ChatMessage = TestConfig.ChatMessage,
                };

                var sendData = PacketToBytes.Make(PACKETID.REQ_ROOM_CHAT, packet);
                var sendError = await clientSocket.SendAsync(sendData.Length, sendData);
                if (sendError != "")
                {
                    return End(dummy, false, sendError);
                }

                dummy.IncreasePacket(PACKETID.REQ_ROOM_CHAT);

                return await RecvProc(dummy);
            }
            catch (Exception ex)
            {
                return Utils.MakeResult(dummy.Index, false, ex.ToString(), this);
            }
        }

        public override (bool, string) CheckNtfRoomChat(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomChat>(packetBuffer);
            if (body.ChatMessage != ChatMessage)
            {
                return Utils.MakeResult(dummy.Index, false, $"메시지 다름({body.ChatMessage})", this);
            }

            return Utils.MakeResult(dummy.Index, true, "");
        }
    }
}
