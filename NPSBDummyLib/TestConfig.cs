using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib
{
    public class TestConfig
    {
        public string RmoteIP;
        public int RemotePort;

        public TestCase ActionCase;

        public int DummyCount;

        public Int64 RepeatConnectCount; // 지정 값만큼 접속 후 끊기 반복
        public Int64 RepeatConnectDateTimeSec; // 지정 시간 동안 접속 후 끊기 반복. RepeatConnectCount 이 0일 때만 유효 


        public bool CheckConfigValue()
        {
            return false;
        }

    }

   
}
