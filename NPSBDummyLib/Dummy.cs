using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public partial class Dummy
    {
        public Int32 Index { get; private set; }

        public Int32 RoomNumber { get; set; }

        public bool IsRecvWorkerThread { get; set; }

        public AsyncSocket ClientSocket;
        SendPacketInfo SendPacket;
        RecvPacketInfo RecvPacket;
        ManualResetEvent RecvEndCond; 

        public Int64 ConnectCount { get; private set; }

        public bool IsSuccessd { get; private set; } = false;

        string LastExceptionMessage;

        public void Connected() { ++ConnectCount;  }


        public void Init(Int32 index)
        {
            Index = index;
            ClientSocket = new AsyncSocket();
            SendPacket = new SendPacketInfo();
            RecvPacket = new RecvPacketInfo();
            RecvEndCond = new ManualResetEvent(false);
            SendPacket.Init(DummyManager.GetDummyInfo.PacketSizeMax);
            RecvPacket.Init(DummyManager.GetDummyInfo.PacketSizeMax);
        }

        public string GetUserID()
        {
            return $"User{Index}";
        }

        public async Task<(bool Result, string Error)> ConnectAsyncAndReTry(string ip, int port)
        {
            const int maxTryCount = 8;
            
            for (int i = 0; i < maxTryCount; ++i)
            {
                var (result, error) = await ClientSocket.ConnectAsync(ip, port);

                if (result == false)
                {
                    await Task.Delay(64);
                    continue;
                }

                return (result, error);
            }

            return (false, "Fail Connect");
        }

        public void SetSuccess(bool isSuccess)
        {
            IsSuccessd = isSuccess;
        }


#pragma warning disable 1998
        public Int64 DisConnect()
        {
            Int64 currentCount = 0;
            try
            {
                if (ClientSocket.IsConnected())
                {
                    currentCount = ClientSocket.Close();
                    IsRecvWorkerThread = false;
                    RecvEndCond.Set();
                }
                
            }
            catch(Exception ex)
            {
                LastExceptionMessage = ex.Message;
            }
            return currentCount;
        }
#pragma warning restore 1998        
    }
}
