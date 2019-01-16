using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    public class TestResultManager
    {
        Dictionary<Int64, TestResultReport> ResultDic = new Dictionary<long, TestResultReport>();

        public void AddTestResult(Int64 index, TestCase testType, List<Dummy> dummyList)
        {
            var report = new TestResultReport();
            report.Index = index;
            report.Case = testType;
            report.DummyCount = dummyList.Count;

            for(int i = 0; i < report.DummyCount; ++i)
            {
                if (dummyList[i].IsSuccessd)
                {
                    ++report.SuccessCount;
                }
                else
                {
                    ++report.FailCount;
                }
            }
        }
    }

    public class TestResultReport
    {
        public Int64 Index;
        public TestCase Case;
        public int DummyCount;
        public int SuccessCount;
        public int FailCount;
    }
}
