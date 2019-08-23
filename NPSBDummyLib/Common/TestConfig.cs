using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NPSBDummyLib
{
    public class TestConfig
    {
        //public string RmoteIP;
        //public int RemotePort;

        //public int DummyCount;
        //public int PacketSizeMax;
        public int RoomNumber;                 // 방번호

        public Int64 RepeatConnectCount;       // 지정 값만큼 접속 후 끊기 반복
        public Int64 RepeatConnectDateTimeSec; // 지정 시간 동안 접속 후 끊기 반복. RepeatConnectCount 이 0일 때만 유효 
        public Int64 LimitRecvDelayTimeSec;    // 패킷 받기 시간 제한
        public Int32 LimitActionTime;          // 액션에 대한 시간 제한(ms)
        public Int32 ActionIntervalTime;       // 액션간 간격 시간(ms)
        public Int32 ActionRepeatCount;        // 액션 반복 횟수
        public Int32 DummyIntervalTime;        // 더미간 실행 간격(ms)
      
        public int MaxVaildActionRecvCount;    // recv시 유효한 action 최대 수
        public string ChatMessage;             // 채팅 메시지
        public Func<bool> IsConditionFunc;
        public Int64 TestUniqueId;
        public TestCase ActionCase;


        public bool CheckConfigValue()
        {
            return false;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            var bindingFlags = BindingFlags.Instance |
                               BindingFlags.Public;

            var fieldNames = typeof(TestConfig).GetFields()
                .Select(field => field.Name)
                .ToList();

            var fieldValues = GetType()
                .GetFields(bindingFlags)
                .Select(field => field.GetValue(this))
                .ToList();

            for (var idx = 0; idx < fieldNames.Count(); ++idx)
            {
                result.Append($"{fieldNames[idx]} : {fieldValues[idx]}\n");
            }

            return result.ToString();
        }
    }
}
