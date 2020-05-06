using CSBaseLib;
using MessagePack;

namespace NPSBDummyLib
{
    public partial class ActionBase
    {
        public void RegistCommonPacket()
        {
            RegistRecvFunc(PACKETID.NTF_ROOM_USER_LIST, CheckNtfRoomUserList, false);
            RegistRecvFunc(PACKETID.NTF_ROOM_NEW_USER, CheckNtfRoomNewUser, false);
            RegistRecvFunc(PACKETID.NTF_ROOM_LEAVE_USER, CheckNtfRoomLeaveUser, false);
            RegistRecvFunc(PACKETID.NTF_ROOM_CHAT, CheckNtfRoomChat, false);
        }

        private (bool, string) CheckNtfRoomUserList(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomUserList>(packetBuffer);
            if (!body.UserIDList.Contains(dummy.GetUserID()))
            {
                return Utils.MakeResult(dummy.Index, false, "해당 유저의 아이디 부재");
            }

            return Utils.MakeResult(dummy.Index, true, "");
        }

        public (bool, string) CheckNtfRoomNewUser(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomNewUser>(packetBuffer);
            var sender = TestConfig.GetDummyFunc(dummy.Index);
            if (sender == null)
            {
                return Utils.MakeResult(dummy.Index, false, $"해당 sender가 존재하지 않음({dummy.Index})");
            }

            if (body.UserID == sender.GetUserID())
            {
                return Utils.MakeResult(dummy.Index, false, $"자신에게 노티됨({body.UserID})");
            }

            return Utils.MakeResult(dummy.Index, true, "");
        }

        private (bool, string) CheckNtfRoomLeaveUser(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomLeaveUser>(packetBuffer);
            if (body.UserID == dummy.GetUserID())
            {
                return Utils.MakeResult(dummy.Index, false, $"자신에게 노티됨({body.UserID})");
            }

            return Utils.MakeResult(dummy.Index, true, "");
        }

        public virtual (bool, string) CheckNtfRoomChat(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomChat>(packetBuffer);

            return Utils.MakeResult(dummy.Index, true, "");
        }
    }
}
