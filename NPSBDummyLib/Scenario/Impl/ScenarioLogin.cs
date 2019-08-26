using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public class ScenarioLogin : ScenarioBase
    {
        public override async Task TaskAsync(Dummy dummy, TestConfig config)
        {
            var connect = MakeActionFactory(TestCase.ACTION_CONNECT, config);
            var login = MakeActionFactory(TestCase.ACTION_LOGIN, config);

            ResultList.Add(await connect.Run(dummy));

            //await Task.Delay(config.ActionIntervalTime);

            ResultList.Add(await login.Run(dummy));


        }
    }
}
