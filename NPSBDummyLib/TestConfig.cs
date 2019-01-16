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

        public int RepeatConnectCount; // 지정 값만큼 접속 후 끊기 반복 횟수. 0 이상 가능. 0 이면 접속 후 대기, 
                      

        public bool CheckConfigValue()
        {
            return false;
        }

    }

    public enum TestCase
    {
        ONLY_CONNECT = 1,
        REPEAT_CONNECT = 2,
        ECHO = 3,
        ECHO_CONNECT_DISCONNECT = 4,
        ECHO_CONNECT_DISCONNECT_RANDOM = 5,
        ECHO_CONNECT_DISCONNECT_SERVER = 6,
    }
}
