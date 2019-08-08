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
            report.UniqueId = index;
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

            ResultDic.Add(index, report);
        }

        public List<string> WriteTestResult(Int64 testUniqueId, TestConfig testConfig)
        {
            var resultStringList = new List<string>();

            var result = ResultDic.TryGetValue(testUniqueId, out var report);
            if (result == false)
            {
                return resultStringList;
            }
                        
            switch(report.Case)
            {
                case TestCase.ONLY_CONNECT:
                    {
                        resultStringList.Add($"[TestCase - {report.Case}]");
                        resultStringList.Add($"DummyCount:{report.DummyCount}, Success:{report.SuccessCount}, Fail:{report.FailCount}");
                    }
                    break;
                case TestCase.REPEAT_CONNECT:
                    {
                        resultStringList.Add($"[TestCase - {report.Case}, CondiCount:{testConfig.RepeatConnectCount} or CondiTime:{testConfig.RepeatConnectDateTimeSec} Sec]");
                        resultStringList.Add($"DummyCount:{report.DummyCount}, Success:{report.SuccessCount}, Fail:{report.FailCount}");
                    }
                    break;
            }

            return resultStringList;
        }

    } // end Class

    public class TestResultReport
    {
        public Int64 UniqueId;
        public TestCase Case;
        public int DummyCount;
        public int SuccessCount;
        public int FailCount;
    }
}
