using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Dummy
{
    class Dummy
    {
        AsyncSocket ClientSocket = new AsyncSocket();

        SendPacketInfo SendPacket = new SendPacketInfo();
        RecvPacketInfo RecvPacket = new RecvPacketInfo();

        public Action<string> MsgFunc; //[진행중] [완료] [실패]
        public Action<string> LogFunc; //[진행중] [완료] [실패]


        public async Task<bool> ConnectAsync(string ip, int port, int tryCount)
        {
            for(int i = 0; i < tryCount; ++i)
            {
                var result = await ClientSocket.ConnectAsync(ip, port);

                //TODO: result를 로그로 남기도록 한다.

                if (string.IsNullOrEmpty(result))
                {
                    return true;
                }

                //동시 접속이 너무 많아서 서버가 받지 못하는 상황일 수 있으니 잠시 대기 후 다시 접속을 시도한다
            }

            return true;
        }

#pragma warning disable 1998
        public async Task EndAsync()
        {
            ClientSocket.Close();
        }
#pragma warning restore 1998

    }
}
