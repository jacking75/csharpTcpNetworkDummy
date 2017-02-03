using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

using TcpTapClientSocketLib;
using System.Threading;

namespace TcpDummyClientsLib
{
    // 아래 항목을 랜던하게 선택하게 한다 
    // 1)데이터 보내고 받기, 2)접속 끊기, 3)데이터 보내고 바로 끊기(서버로 부터 받지 않는다)
    public class ModuleRepeatConnDisConnAndSendData : ModuleRepeatConnDisConn
    {
        async Task<string> ReConnect(AsyncTcpSocketClient client, int repeatCount, DateTime repeatTime)
        {
            var rand = new Random();

            try
            {
                int workingCount = 0;

                while (true)
                {
                    if (Interlocked.Read(ref IsStart) == (Int64)Status.STOP)
                    {
                        return "중단";
                    }

                    if (Interlocked.Read(ref IsStart) == (Int64)Status.PAUSE)
                    {
                        await Task.Delay(1);
                    }

                    if (repeatCount != 0 && repeatCount == workingCount)
                    {
                        break;
                    }

                    if (repeatTime >= DateTime.Now)
                    {
                        break;
                    }

                    await ConnectDisconnectSend(client, rand);

                    ++workingCount;
                }
            }
            catch (Exception ex)
            {
                return "에러:" + ex.Message;
            }

            return "완료";
        }


        // 1)데이터 보내고 받기, 2)접속 끊기, 3)데이터 보내고 바로 끊기(서버로 부터 받지 않는다)
        async Task<string> ConnectDisconnectSend(AsyncTcpSocketClient client, Random rand)
        {
            try
            {
                var sel = rand.Next(0, 3);

                if (client.State == TcpSocketConnectionState.Closed)
                {
                    await client.Connect();
                }

                switch(sel)
                {
                    case 0:
                        {
                            var data = MakePacket();
                            await client.SendAsync(data);

                            // 패킷 처리 루틴에서 접속을 끊는다
                        }
                        break;
                    case 1:
                        {
                            await client.Close();
                        }
                        break;
                    case 2:
                        {
                            var data = MakePacket();
                            await client.SendAsync(data);

                            await client.Close();
                        }
                        break;
                }                
            }
            catch (Exception ex)
            {
                return "에러:" + ex.Message;
            }

            return null;
        }


        public Random RandDataSize = new Random();

        public byte[] MakePacket()
        {
            var length = RandDataSize.Next(32, 512);
            var text = Utils.RandomString(length);


            Int16 packetId = 241;
            var textLen = (Int16)Encoding.Unicode.GetBytes(text).Length;
            var bodyLen = (Int16)(textLen + 2);

            var sendData = new byte[4 + 2 + textLen];
            Buffer.BlockCopy(BitConverter.GetBytes(packetId), 0, sendData, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(bodyLen), 0, sendData, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(textLen), 0, sendData, 4, 2);
            Buffer.BlockCopy(Encoding.Unicode.GetBytes(text), 0, sendData, 6, textLen);

            return sendData;
        }
    }
}
