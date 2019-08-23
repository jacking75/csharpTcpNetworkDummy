using CSBaseLib;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public partial class ActionBase
    {
        public delegate (int, bool, string) RecvFunc(Dummy dummy, PACKETID packetId, byte[] packetBuffer);


        public TestCase TestType { get; private set; }
        public TestConfig TestConfig { get; private set; }

        //public Action<string> MsgFunc; //[진행중] [완료] [실패]
        public HashSet<PACKETID> CheckPacketIDSet = new HashSet<PACKETID>();
        protected DummyManager DummyManager;
        protected Dictionary<PACKETID, RecvFunc> RecvFuncDic = new Dictionary<PACKETID, RecvFunc>();

        public ActionBase(TestCase testType, DummyManager dummyManager, TestConfig config)
        {
            TestType = testType;
            DummyManager = dummyManager;
            TestConfig = config;

            RegistCommonPacket();
        }

        public void RegistRecvFunc(PACKETID packetId, RecvFunc func, bool isCheck)
        {
            if (func == null)
            {
                return;
            }

            RecvFuncDic.Add(packetId, func);
            if (isCheck)
            {
                CheckPacketIDSet.Add(packetId);
            }
        }

        public void RegistNfyRecvFunc(PACKETID packetId, RecvFunc func)
        {
            RegistRecvFunc(packetId, func, false);
        }


        public RecvFunc GetRecvFunction(PACKETID packetId)
        {
            RecvFunc func = null;
            if (CheckPacketIDSet.Contains(packetId))
            {
                CheckPacketIDSet.Remove(packetId);
            }

            RecvFuncDic.TryGetValue(packetId, out func);
            return func;
        }

        public bool IsCompleteRecv()
        {
            return (CheckPacketIDSet.Count == 0);
        }


        async public Task<(int, bool, string)> Run(Dummy dummy, TestConfig config)
        {
            // 여기에 perf 관련 데이터 추가
            var result = await TaskAsync(dummy, config);

            return result;
        }

        public virtual string GetActionName()
        {
            return "None";
        }

        public virtual void Before(Dummy dummy)
        {
        }

        public virtual void After()
        {
        }

        public (int, bool, string) End(Dummy dummy, bool result, string message)
        {
            if (result)
            {
                dummy.SetSuccess(true);
            }
            else
            {
                dummy.SetSuccess(false);
            }

            var log = $"{GetActionName()} End. DummyIndex:{dummy.Index}, result:{result}, message:{message}";
            Utils.Logger.Debug(log);

            return (dummy.Index, result, log);
        }

        protected virtual async Task<(int, bool, string)> TaskAsync(Dummy dummy, TestConfig config)
        {
            return await Task.Run<(int, bool, string)>(() =>
            {
                return (0, false, "Don't call me.");

            });
        }

        protected async Task<(int, bool, string)> RecvProc(Dummy dummy)
        {
            if (dummy == null)
            {
                return End(dummy, false, $"존재하지 않는 더미");
            }

            var expectTime = DateTime.Now.AddMilliseconds(TestConfig.LimitActionTime);

            do
            {
                var recvResult = await dummy.PopRecvResult(TestConfig.LimitActionTime);
                (EResultCode errorCode, PACKETID packetId, byte[] packetBuffer) = recvResult;

                if (expectTime < DateTime.Now)
                {
                    return End(dummy, false, "시간 초과");
                }

                if (errorCode != EResultCode.RESULT_OK)
                {
                    return End(dummy, false, $"에러 발생:{errorCode}");
                }

                var func = GetRecvFunction(packetId);
                if (func == null)
                {
                    return End(dummy, false, $"유효하지 않은 패킷({packetId})");
                }

                if (DummyManager.GetDummyInfo.IsRecvDetailProc)
                {
                    var (index, result, message) = func(dummy, packetId, packetBuffer);
                    if (!result)
                    {
                        return End(dummy, false, message);
                    }
                }

            } while (!IsCompleteRecv());

            return End(dummy, true, "ok");
        }
    }
}
