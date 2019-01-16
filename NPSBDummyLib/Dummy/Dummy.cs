using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Dummy
{
    public class Dummy
    {
        public AsyncSocket ClientSocket = new AsyncSocket();

        SendPacketInfo SendPacket = new SendPacketInfo();
        RecvPacketInfo RecvPacket = new RecvPacketInfo();

        public Int64 ConnectCount { get; private set; }

        int ConnecTryCount;
                        

        public void Connected() { ++ConnectCount;  }


        public async Task<(bool Result, string Error)> ConnectAsyncAndReTry(string ip, int port)
        {
            const int maxTryCount = 8;

            for (int i = 0; i < maxTryCount; ++i)
            {
                var (result, error) = await ClientSocket.ConnectAsync(ip, port);

                if (result == false)
                {
                    await Task.Delay(64);
                }

                return (result, error);
            }

            return (false, "Fail Connect");
        }


#pragma warning disable 1998
        public async Task DisConnectAsync()
        {
            ClientSocket.Close();
        }
#pragma warning restore 1998        
    }
}
