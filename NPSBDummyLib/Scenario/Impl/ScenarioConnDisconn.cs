using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    class ScenarioLoginOut : ScenarioBase
    {
        public override async Task TaskAsync(Dummy dummy, TestConfig config)
        {
            var connect = MakeActionFactory(TestCase.ACTION_CONNECT, config);
            var disconnect = MakeActionFactory(TestCase.ACTION_DISCONNECT, config);

            for (var idx = 0; idx < config.RepeatConnectCount; idx++)
            {
                ResultList.Add(await connect.Run(dummy));

                //await Task.Delay(config.ActionIntervalTime);

                ResultList.Add(await disconnect.Run(dummy));
            }

        
        }
    }
}
