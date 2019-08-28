using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public abstract class ScenarioBase
    {
        public abstract Task<(bool, string)> TaskAsync(Dummy dummy, TestConfig config);

        public abstract string GetScenarioName();
    }
}
