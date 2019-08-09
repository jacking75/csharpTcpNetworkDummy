using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public class Dummy
    {
        public Int32 Index { get; private set; }

        public AsyncSocket ClientSocket = new AsyncSocket();

        SendPacketInfo SendPacket = new SendPacketInfo();
        RecvPacketInfo RecvPacket = new RecvPacketInfo();

        public Int64 ConnectCount { get; private set; }

        public bool IsSuccessd { get; private set; } = false;

        string LastExceptionMessage;
                        

        public void Connected() { ++ConnectCount;  }


        public void Init(Int32 index)
        {
            Index = index;
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
        public void DisConnect()
        {
            try
            {
                ClientSocket.Close();
            }
            catch(Exception ex)
            {
                LastExceptionMessage = ex.Message;
            }
        }
#pragma warning restore 1998        
    }
}
