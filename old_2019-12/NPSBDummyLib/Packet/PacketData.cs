using MessagePack; //https://github.com/neuecc/MessagePack-CSharp
using System;
using System.Collections.Generic;


namespace CSBaseLib
{
    public class PacketDef
    {
        public const Int16 PACKET_HEADER_SIZE = 5;
        public const int INVALID_ROOM_NUMBER = -1;
    }

    // 로그인 요청
    [MessagePackObject]
    public class PKTReqLogin
    {
        [Key(0)]
        public string UserID;
        [Key(1)]
        public string AuthToken;
    }

    [MessagePackObject]
    public class PKTResLogin
    {
        [Key(0)]
        public short Result;
    }


    [MessagePackObject]
    public class PKNtfMustClose
    {
        [Key(0)]
        public short Result;
    }


    [MessagePackObject]
    public class PKTReqRoomEnter
    {
        [Key(0)]
        public int RoomNumber;
    }

    [MessagePackObject]
    public class PKTResRoomEnter
    {
        [Key(0)]
        public short Result;
    }

    [MessagePackObject]
    public class PKTNtfRoomUserList
    {
        [Key(0)]
        public List<string> UserIDList = new List<string>();
    }

    [MessagePackObject]
    public class PKTNtfRoomNewUser
    {
        [Key(0)]
        public string UserID;
    }


    [MessagePackObject]
    public class PKTReqRoomLeave
    {
    }

    [MessagePackObject]
    public class PKTResRoomLeave
    {
        [Key(0)]
        public short Result;
    }

    [MessagePackObject]
    public class PKTNtfRoomLeaveUser
    {
        [Key(0)]
        public string UserID;
    }


    [MessagePackObject]
    public class PKTReqRoomChat
    {
        [Key(0)]
        public string ChatMessage;
    }


    [MessagePackObject]
    public class PKTNtfRoomChat
    {
        [Key(0)]
        public string UserID;

        [Key(1)]
        public string ChatMessage;
    }
}
