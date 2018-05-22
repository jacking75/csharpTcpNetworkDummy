using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.Dummy
{
    public partial class DummyManager
    {
        public async Task<(bool,string)> RepeatConnectAsync(int dummyIndex)
        {
            var expectConnectCount = Config.RepeatConnectCount + 1;
            var dummy = DummyList[dummyIndex];

            for (int i = 0; i <= expectConnectCount; ++i)
            {
                await Task.Run(async () => await dummy.ConnectAsyncAndReTry(Config.RmoteIP, Config.RemotePort));
                dummy.Connected();

                if (expectConnectCount != 1)
                {
                    await dummy.DisConnectAsync();
                } 
                else
                {
                    //TODO:결과를 통보한다
                }
            }

            if (dummy.ClientSocket.IsConnected())
            {
                while(true)
                {
                    await Task.Delay(500);
                    
                    //TODO: 종료 통보가 오면 종료한다
                    if(IsStart == false)
                    {
                        break;
                    }
                }
            }

            return (true, "");
        }
    }
}
