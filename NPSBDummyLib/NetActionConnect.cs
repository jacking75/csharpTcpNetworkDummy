using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public class NetActionConnect
    {
        public static async Task<(bool, string)> ConnectOnlyAsync(Dummy dummy, TestConfig config, Func<bool> isContinue/*int dummyIndex*/)
        {
            //var dummy = DummyList[dummyIndex];

            var result = await Task.Run(async () => await dummy.ConnectAsyncAndReTry(config.RmoteIP, config.RemotePort));

            if(result.Result == false)
            {
                return (false, result.Error);
            }
            else
            {
                dummy.Connected();
            }
            
                                    
            while (true)
            {
                await Task.Delay(500);

                if (isContinue() == false)
                {
                    break;
                }
            }
            
            return (true, "");
        }


        public static async Task<(bool,string)> RepeatConnectAsync(Dummy dummy, TestConfig config, Func<bool> isContinue/*int dummyIndex*/)
        {
            //var expectConnectCount = Config.RepeatConnectCount + 1;
            //var dummy = DummyList[dummyIndex];
            var expectConnectCount = config.RepeatConnectCount + 1;

            for (int i = 0; i <= expectConnectCount; ++i)
            {
                await Task.Run(async () => await dummy.ConnectAsyncAndReTry(config.RmoteIP, config.RemotePort));
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
                    if(isContinue() == false)
                    {
                        break;
                    }
                }
            }

            return (true, "");
        }


    } // end Class
}
