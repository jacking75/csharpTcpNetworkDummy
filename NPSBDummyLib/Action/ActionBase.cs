using CSBaseLib;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NPSBDummyLib
{
    public partial class ActionBase
    {
        public delegate (bool, string) RecvFunc(Dummy dummy, PACKETID packetId, byte[] packetBuffer);

        public TestCase TestType { get; private set; }
        public TestConfig TestConfig { get; private set; }

        public List<PACKETID> CheckPacketIDList = new List<PACKETID>();
        public HashSet<PACKETID> CheckPacketIDSet = new HashSet<PACKETID>();
        protected Dictionary<PACKETID, RecvFunc> RecvFuncDic = new Dictionary<PACKETID, RecvFunc>();

        public ActionBase(TestCase testType, TestConfig config)
        {
            TestType = testType;
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
                CheckPacketIDList.Add(packetId);
            }
        }

        public void RegistNfyRecvFunc(PACKETID packetId, RecvFunc func)
        {
            RegistRecvFunc(packetId, func, false);
        }

        public RecvFunc GetRecvFunction(PACKETID packetId)
        {
            if (CheckPacketIDSet.Contains(packetId))
            {
                CheckPacketIDSet.Remove(packetId);
            }

            if (!RecvFuncDic.TryGetValue(packetId, out var func))
            {
                func = null;
            }
            return func;
        }

        public bool IsCompleteRecv()
        {
            return (CheckPacketIDSet.Count == 0);
        }


        async public Task<(bool, string)> Run(Dummy dummy)
        {
            Before(dummy);

            // 여기에 perf 관련 데이터 추가
            var result = await TaskAsync(dummy);

            if (TestConfig.ActionIntervalTime > 0)
            {
                await Task.Delay(TestConfig.ActionIntervalTime);
            }

            After();

            return result;
        }

        public virtual string GetActionName()
        {
            return "None";
        }

        public virtual void Before(Dummy dummy)
        {
            foreach(var packetId in CheckPacketIDList)
            {
                CheckPacketIDSet.Add(packetId);
            }
        }

        public virtual void After()
        {
        }

        public (bool, string) End(Dummy dummy, bool result, string message)
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

            return (result, log);
        }

        protected virtual async Task<(bool, string)> TaskAsync(Dummy dummy)
        {
            return await Task.Run<(bool, string)>(() =>
            {
                return (false, "Don't call me.");

            });
        }

        protected async Task<(bool, string)> RecvProc(Dummy dummy)
        {
            if (dummy == null)
            {
                return End(dummy, false, $"존재하지 않는 더미");
            }

            var expectTime = DateTime.Now.AddMilliseconds(TestConfig.LimitActionTime);

            do
            {
                var recvResult = await dummy.PopRecvResult(TestConfig.LimitActionTime);
                var errorCode = recvResult.ResultCode;
                var packetId = recvResult.PacketId;
                var packetBuffer = recvResult.BodyBytes;
                
                using (recvResult)
                {
                    if (expectTime < DateTime.Now)
                    {
                        return End(dummy, false, "시간 초과");
                    }

                    var func = GetRecvFunction(packetId);
                    if (func == null)
                    {
                        if (errorCode != EResultCode.RESULT_OK)
                        {
                            return End(dummy, false, $"에러 발생:{errorCode}");
                        }

                        return End(dummy, false, $"유효하지 않은 패킷({packetId})");
                    }

                    if (DummyManager.GetDummyInfo.IsRecvDetailProc || errorCode != EResultCode.RESULT_OK)
                    {
                        var (result, message) = func(dummy, packetId, packetBuffer);
                        if (!result)
                        {
                            return End(dummy, false, message);
                        }
                    }
                }

            } while (!IsCompleteRecv());

            return End(dummy, true, "ok");
        }
    }
}
