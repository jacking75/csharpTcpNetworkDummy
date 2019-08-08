using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ActionEcho
    {
        AsyncSocket ClientSocket = new AsyncSocket();

        SendPacketInfo SendPacket = new SendPacketInfo();
        RecvPacketInfo RecvPacket = new RecvPacketInfo();

        //public Action<string> MsgFunc; //[진행중] [완료] [실패]

        public async Task<(bool, string)> EchoAsync(EchoCondition cond)
        {
            SendPacket.Init(cond.PacketSizeMax);
            RecvPacket.Init(cond.PacketSizeMax);

            try
            {
                var (result, error) = await ClientSocket.ConnectAsync(cond.IP, cond.Port);
                if (result == false)
                {
                    return (false, error);
                }

                int curEchoCount = 0;

                while (true)
                {
                    //TODO 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                    SendPacket.SetData(cond.PacketSizeMin, cond.PacketSizeMax);
                    var sendError = await ClientSocket.SendAsync(SendPacket.BufferSize, SendPacket.BufferData);
                    if (sendError != "")
                    {
                        return End(false, sendError);
                    }

                    //TODO 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                    var (recvCount, recvError) = await ClientSocket.ReceiveAsync(RecvPacket.BufferSize, RecvPacket.Buffer);
                    if (recvError != "")
                    {
                        return End(false, recvError);
                    }

                    if (recvCount > 0)
                    {
                        RecvPacket.Received(recvCount);
                    }
                    else if (recvCount == 0)
                    {
                        return End(false, "연결 종료");
                    }


                    if (SendPacket.BodyData() != RecvPacket.BodyData())
                    {
                        return End(false, "데이터 틀림");
                    }


                    ++curEchoCount;

                    if (cond.IsEnd(curEchoCount))
                    {
                        break;
                    }
                }

                return End(true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }

//#pragma warning disable 1998
        public (bool,string) End(bool result, string message)
        {
            ClientSocket.Close();
            return (result, message);
        }
//#pragma warning restore 1998


    }


    public class EchoCondition
    {
        public string IP;
        public int Port;
        public int PacketSizeMin;
        public int PacketSizeMax;

        DateTime EchoTime;
        int EchoCount = 0;
                
        public void Set(int echoCount, int echoTiimeSecond)
        {
            if(echoTiimeSecond == 0)
            {
                EchoCount = echoCount;
            }
            else
            {
                EchoTime = DateTime.Now.AddSeconds(echoTiimeSecond);
            }
        }

        public bool IsEnd(int curEchoCount)
        {
            if (EchoCount > 0)
            {
                if (EchoCount == curEchoCount)
                {
                    return true;
                }
            }
            else
            {
                if (EchoTime <= DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
