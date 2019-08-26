using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSBaseLib;

namespace NPSBDummyLib
{
    public class TestResultManager
    {
        Dictionary<Int64, TestResultReport> ResultDic;
        Dictionary<PACKETID, int> PacketStatDic;

        public TestResultManager()
        {
            ResultDic = new Dictionary<long, TestResultReport>();
            PacketStatDic = new Dictionary<PACKETID, int>();
        }

        public void AddTestResult(Int64 index, TestCase testType, List<Dummy> dummyList, DateTime startTime, List<Task<(bool, string)>> testResults = null)
        {
            var isExist = ResultDic.ContainsKey(index);
            var report = isExist ? ResultDic[index] : new TestResultReport();
            report.StartTime = startTime;
            report.UniqueId = index;
            report.Case = testType;
            report.DummyCount = dummyList.Count;
            if (testResults != null )
            {
                report.DetailLog.AddRange(testResults);
            }
            

            for (int i = 0; i < report.DummyCount; ++i)
            {
                var dummy = dummyList[i];

                if (dummy.IsSuccessd)
                {
                    ++report.SuccessCount;
                }
                else
                {
                    ++report.FailCount;
                }

                foreach(var pair  in dummy.GetPacketList())
                {
                    IncreasePacket(pair.Key, pair.Value);
                }
            }

            if (isExist)
            {
                ResultDic[index] = report;
            }
            else
            {
                ResultDic.Add(index, report);
            }
            
        }

        public void AddDetailTestResult(Int64 index, List<Task<(bool, string)>> testResults)
        {
            var isExist = ResultDic.ContainsKey(index);
            var report = isExist ? ResultDic[index] : new TestResultReport();
            report.DetailLog.AddRange(testResults);
            if (isExist)
            {
                ResultDic[index] = report;
            }
            else
            {
                ResultDic.Add(index, report);
            }
        }

        public List<ReportData> WriteTestResult(Int64 testUniqueId, TestConfig testConfig)
        {
            var resultReportList = new List<ReportData>();

            while (ResultDic.Count > 0)
            {
                var result = ResultDic.TryGetValue(testUniqueId, out var report);
                if (result == false)
                {
                    return new List<ReportData>();
                }

                ResultDic.Remove(testUniqueId);
                var testTime = DateTime.Now - report.StartTime;

                switch (report.Case)
                {
                    case TestCase.ONLY_CONNECT:
                        {
                            resultReportList.Add(new ReportData($"[TestCase - {report.Case}]"));
                            resultReportList.Add(new ReportData($"[Test Time - {testTime.TotalMilliseconds} MillSec]"));
                            resultReportList.Add(new ReportData($"DummyCount:{report.DummyCount}, Success:{report.SuccessCount}, Fail:{report.FailCount}", MakeDetailLog(report)));
                        }
                        break;
                    case TestCase.REPEAT_CONNECT:
                        {
                            resultReportList.Add(new ReportData($"[TestCase - {report.Case}, CondiCount:{testConfig.RepeatConnectCount} or CondiTime:{testConfig.RepeatConnectDateTimeSec} Sec]"));
                            resultReportList.Add(new ReportData($"[Test Time - {testTime.TotalMilliseconds} MillSec]"));
                            resultReportList.Add(new ReportData($"DummyCount:{report.DummyCount}, Success:{report.SuccessCount}, Fail:{report.FailCount}", MakeDetailLog(report)));
                        }
                        break;
                    case TestCase.ECHO:
                        {
                            resultReportList.Add(new ReportData($"[TestCase - {report.Case}]"));
                            resultReportList.Add(new ReportData($"[Test Time - {testTime.TotalMilliseconds} MillSec]"));
                            resultReportList.Add(new ReportData($"DummyCount:{report.DummyCount}, Success:{report.SuccessCount}, Fail:{report.FailCount}", MakeDetailLog(report)));
                        }
                        break;

                    default:
                        {
                            resultReportList.Add(new ReportData($"[TestCase - {report.Case}]"));
                            resultReportList.Add(new ReportData($"[Test Time - {testTime.TotalMilliseconds} MillSec]"));
                            resultReportList.Add(new ReportData($"DummyCount:{report.DummyCount}, Success:{report.SuccessCount}, Fail:{report.FailCount}", MakeDetailLog(report)));
                        }
                        break;
                }
            }

            return resultReportList;
        }

        public string MakeDetailLog(TestResultReport report)
        {
            if (report.DetailLog == null)
            {
                return null;
            }

            StringBuilder resultMessage = new StringBuilder();
            foreach (var detailLog in report.DetailLog)
            {
                (var success, var message) = detailLog.Result;
                var result = success ? "" : " => failed";
                resultMessage.Append($"{message}{result}\n");
            }

            return resultMessage.ToString();
        }

        public void IncreasePacket(PACKETID packetId, int count)
        {
            if (PacketStatDic.ContainsKey(packetId))
            {
                PacketStatDic[packetId] += count;
            }
            else
            {
                PacketStatDic.Add(packetId, count);
            }
        }

        public string MakePacketStat()
        {
            StringBuilder result = new StringBuilder();
            Int32 totalCount = 0;

            foreach (var pair in PacketStatDic)
            {
                string packetID = pair.Key.ToString();
                string count = pair.Value.ToString();

                result.Append($"{packetID} => {count}\n");
                totalCount += pair.Value;
            }

            result.Append($"Total => {totalCount}\n");

            return result.ToString();
        }

        public void Clear()
        {
            ResultDic.Clear();
            PacketStatDic.Clear();
        }

    } // end Class

    public class TestResultReport
    {
        public DateTime StartTime;
        public Int64 UniqueId;
        public TestCase Case;
        public int DummyCount;
        public int SuccessCount;
        public int FailCount;
        public List<Task<(bool, string)>> DetailLog;
        public TestResultReport()
        {
            DetailLog = new List<Task<(bool, string)>>();
        }
    }
}
