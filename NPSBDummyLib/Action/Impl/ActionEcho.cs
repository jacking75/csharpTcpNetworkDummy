using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ActionEcho
    {
        //AsyncSocket ClientSocket = new AsyncSocket();

        SendPacketInfo SendPacket = new SendPacketInfo();
        RecvPacketInfo RecvPacket = new RecvPacketInfo();

        //public Action<string> MsgFunc; //[진행중] [완료] [실패]

        public async Task<(bool, string)> EchoAsync(Dummy dummy, EchoCondition cond)
        {
            var clientSocket = dummy.ClientSocket;
            SendPacket.Init(cond.PacketSizeMax);
            RecvPacket.Init(cond.PacketSizeMax);

            int curEchoCount = 0;

            try
            {
                var (result, error) = await clientSocket.ConnectAsync(cond.IP, cond.Port);
                if (result == false)
                {
                    return (false, error);
                }

                while (true)
                {
                    // 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                    //Utils.Logger.Debug($"Echo-Send. ClientIndex: {dummy.Index}");

                    SendPacket.SetData(cond.PacketSizeMin, cond.PacketSizeMax);
                    var sendError = await clientSocket.SendAsync(SendPacket.BufferSize, SendPacket.BufferData);
                    if (sendError != "")
                    {
                        return End(dummy, curEchoCount, false, sendError);
                    }


                    // 스레드 잘 사용하는지 알기 위해 스레드 번호찍기
                    //Utils.Logger.Debug($"Echo-Recv. ClientIndex: {dummy.Index}");

                    var (recvCount, recvError) = await clientSocket.ReceiveAsync(RecvPacket.BufferSize, RecvPacket.RecvBuffer);
                    if (recvError != "")
                    {
                        return End(dummy, curEchoCount, false, recvError);
                    }

                    if (recvCount > 0)
                    {
                        RecvPacket.Received(recvCount);
                    }
                    else if (recvCount == 0)
                    {
                        return End(dummy, curEchoCount, false, "연결 종료");
                    }


                    if (SendPacket.BodyData() != RecvPacket.BodyData())
                    {
                        return End(dummy, curEchoCount, false, "데이터 틀림");
                    }


                    ++curEchoCount;

                    if (cond.IsEnd(curEchoCount))
                    {
                        break;
                    }
                }

                return End(dummy, curEchoCount, true, "");
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());
            }
        }

        public (bool,string) End(Dummy dummy, Int32 actionCount, bool result, string message)
        {
            if(result)
            {
                dummy.SetSuccess(true);
            }
            else
            {
                dummy.SetSuccess(false);
            }

            Utils.Logger.Debug($"EchoTest End. DummyIndex:{dummy.Index}, echoCount:{actionCount}, result:{result}, message:{message}");

            dummy.ClientSocket.Close();
            return (result, message);
        }
    }


    public class EchoCondition
    {
        public string IP;
        public int Port;
        public int PacketSizeMin;
        public int PacketSizeMax;

        public DateTime EchoTime { get; private set; }
        public int EchoCount { get; private set; }

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
