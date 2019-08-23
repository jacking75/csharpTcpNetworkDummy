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

        private (int, bool, string) CheckNtfRoomUserList(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomUserList>(packetBuffer);
            if (!body.UserIDList.Contains(dummy.GetUserID()))
            {
                return (dummy.Index, false, "해당 유저의 아이디 부재");
            }

            return (dummy.Index, true, "");
        }

        public (int, bool, string) CheckNtfRoomNewUser(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomNewUser>(packetBuffer);
            var sender = DummyManager.GetDummy(dummy.Index);
            if (sender == null)
            {
                return (dummy.Index, false, $"해당 sender가 존재하지 않음({dummy.Index})");
            }

            if (body.UserID == sender.GetUserID())
            {
                return (dummy.Index, false, $"자신에게 노티됨({body.UserID})");
            }

            return (dummy.Index, true, "");
        }

        private (int, bool, string) CheckNtfRoomLeaveUser(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomLeaveUser>(packetBuffer);
            if (body.UserID == dummy.GetUserID())
            {
                return (dummy.Index, false, $"자신에게 노티됨({body.UserID})");
            }

            return (dummy.Index, true, "");
        }

        virtual public (int, bool, string) CheckNtfRoomChat(Dummy dummy, PACKETID packetId, byte[] packetBuffer)
        {
            var body = MessagePackSerializer.Deserialize<PKTNtfRoomChat>(packetBuffer);

            return (dummy.Index, true, "");
        }

    }
}
