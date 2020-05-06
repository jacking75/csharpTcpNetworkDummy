using System;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    internal class ActionNetConnect
    {
        public static async Task<(bool, string)> ConnectOnlyAsync(Dummy dummy, TestConfig config, Func<bool> isContinue)
        {
            var result = await dummy.ConnectAsyncAndReTry(DummyManager.GetDummyInfo.RmoteIP, DummyManager.GetDummyInfo.RemotePort);

            if(result.Result == false)
            {
                return (false, result.Error);
            }
            else
            {
                dummy.Connected();
            }
            
            if(config.ActionCase == TestCase.ONLY_CONNECT)
            {
                dummy.SetSuccess(true);
                return (true, "");
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


        public static async Task<(bool,string)> RepeatConnectAsync(Dummy dummy, TestConfig config, Func<bool> isContinue)
        {
            var expectConnectCount = config.RepeatConnectCount;
            var expectDateTime = DateTime.Now.AddSeconds(config.RepeatConnectDateTimeSec);

            while (isContinue())
            {
                var result = await dummy.ConnectAsyncAndReTry(DummyManager.GetDummyInfo.RmoteIP, DummyManager.GetDummyInfo.RemotePort);
                if (result.Result == false)
                {
                    return (false, result.Error);
                }


                dummy.Connected();
                dummy.DisConnect();

                if (expectConnectCount > 0 && expectConnectCount == dummy.ConnectCount)
                {
                    dummy.SetSuccess(true);
                    break;
                } 
                else if(expectConnectCount == 0 && expectDateTime <= DateTime.Now)
                {
                    dummy.SetSuccess(true);
                    break;
                }
            }
                        
            return (true, "");
        }


    } // end Class
}
