using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Dummys
{
    class Echo
    {
        AsyncSocket ClientSocket = new AsyncSocket();

        SendPacketInfo SendPacket = new SendPacketInfo();
        RecvPacketInfo RecvPacket = new RecvPacketInfo();

        public Action<string> MsgFunc; //[진행중] [완료] [실패]

        public async Task<string> ProcessAsync(EchoCondition cond)
        {
            SendPacket.Init(cond.PacketSizeMax);
            RecvPacket.Init(cond.PacketSizeMax);

            try
            {
                var result = await ClientSocket.ConnectAsync(cond.IP, cond.Port);
                if (result != "")
                {
                    return result;
                }

                int curEchoCount = 0;

                while (true)
                {
                    SendPacket.SetData(cond.PacketSizeMin, cond.PacketSizeMax);
                    var sendError = await ClientSocket.SendAsync(SendPacket.BufferSize, SendPacket.BufferData);
                    if (sendError != "")
                    {
                        return sendError;
                    }


                    var (recvCount, recvError) = await ClientSocket.ReceiveAsync(RecvPacket.BufferSize, RecvPacket.Buffer);
                    if (recvError != "")
                    {
                        return recvError;
                    }

                    if (recvCount > 0)
                    {
                        RecvPacket.Received(recvCount);
                    }
                    else if (recvCount == 0)
                    {
                        return "연결 종료";
                    }


                    if (SendPacket.BodyData() != RecvPacket.BodyData())
                    {
                        return "데이터 틀림";
                    }


                    ++curEchoCount;

                    if (cond.IsEnd(curEchoCount))
                    {
                        break;
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

#pragma warning disable 1998
        public async Task<string> EndAsync()
        {
            ClientSocket.Close();
            return "";
        }
#pragma warning restore 1998


    }


}
