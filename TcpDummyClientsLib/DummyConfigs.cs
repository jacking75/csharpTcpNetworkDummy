using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpDummyClientsLib
{
    public struct TestConfig
    {
        public int DummyCount;
        public string RemoteIP;
        public UInt16 RemotePort;
    }

    public struct TestRepeatConnDisConnConfig
    {
        public int DummyCount;
        public string RemoteIP;
        public UInt16 RemotePort;

        public int RepeatCount;
        public DateTime RepeatTime;
    }

    public struct TestSimpleEchoConfig
    {
        public int DummyCount;
        public string RemoteIP;
        public UInt16 RemotePort;

        public int RepeatCount;
        public DateTime RepeatTime;
        public int MinSendDataSize;
        public int MaxSendDataSize;


        public Action<string> LogFunc;
    }
}
