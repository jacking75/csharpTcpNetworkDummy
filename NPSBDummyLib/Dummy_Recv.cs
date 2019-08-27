using CSBaseLib;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace NPSBDummyLib
{
    public partial class Dummy
    {
        Task WorkerThread;
        Dictionary<PACKETID, int> PacketStatDic = new Dictionary<PACKETID, int>();
        Channel<TaskCompletionSource<ObjectComponentPacket>> RecvResultChannel = Channel.CreateUnbounded<TaskCompletionSource<ObjectComponentPacket>>();

        public void IncreasePacket(PACKETID packetId)
        {
            if (PacketStatDic.ContainsKey(packetId))
            {
                ++PacketStatDic[packetId];
            }
            else
            {
                PacketStatDic.Add(packetId, 1);
            }
        }

        public List<KeyValuePair<PACKETID, int>> GetPacketList()
        {
            var result =  PacketStatDic.ToList();
            PacketStatDic.Clear();
            return result;
        }

        public async Task EnqueueResult(ObjectComponentPacket result)
        {
            var tcs = new TaskCompletionSource<ObjectComponentPacket>(TaskCreationOptions.RunContinuationsAsynchronously);
            tcs.SetResult(result);
            await RecvResultChannel.Writer.WriteAsync(tcs);
            await tcs.Task;
        }

        public async Task<ObjectComponentPacket> PopRecvResult(int limitActionTime)
        {
            var cts = new CancellationTokenSource(limitActionTime);
            await RecvResultChannel.Reader.WaitToReadAsync(cts.Token);

            if (!RecvResultChannel.Reader.TryRead(out var resultTask))
            {
                var packetObj = ObjectPool<ObjectComponentPacket>.GetInstance.Get();
                packetObj.ResultCode = EResultCode.RESULT_FAILED_POP_CHANNEL;
                return packetObj;
            }

            return resultTask.Task.Result;
        }
        
        public void CreateRecvWorker()
        {
            WorkerThread = Task.Run(async () => {
                IsRecvWorkerThread = true;
                while (IsRecvWorkerThread)
                {
                    var (recvCount, recvError) = await ClientSocket.ReceiveAsync(RecvPacket.BufferSize, RecvPacket.RecvBuffer);

                    var result = await RecvProc(recvCount, recvError);
                    if (!result)
                    {
                        RecvEndCond.WaitOne();
                        RecvEndCond.Reset();
                        
                        if (IsRecvWorkerThread)
                        {
                            DummyManager.DummyDisConnected();
                            await ConnectAsyncAndReTry(DummyManager.GetDummyInfo.RmoteIP, DummyManager.GetDummyInfo.RemotePort);
                            continue;
                        }
                        
                        break;
                    }
                }
            });
        }

        public async Task<bool> RecvProc(int recvCount, string recvError)
        {
            if (recvError != "")
            {
                RecvEndCond.Set();
                if (DummyManager.IsInProgress())
                {
                    var packetObj = ObjectPool<ObjectComponentPacket>.GetInstance.Get();
                    packetObj.ResultCode = EResultCode.RESULT_RECV_ERROR;
                    await EnqueueResult(packetObj);
                    return false;
                }
                else
                {
                    var packetObj = ObjectPool<ObjectComponentPacket>.GetInstance.Get();
                    packetObj.ResultCode = EResultCode.RESULT_OK;
                    await EnqueueResult(packetObj);
                    return false;
                }
            }

            if (recvCount == 0)
            {
                var packetObj = ObjectPool<ObjectComponentPacket>.GetInstance.Get();
                packetObj.ResultCode = EResultCode.RESULT_CONNECTION_EXPIRED;
                packetObj.PacketId = PACKETID.CS_END;
                await EnqueueResult(packetObj);
                return false;
            }

            recvCount = RecvPacket.CombineRemainBuffer(recvCount);

            var packetSize = 0;

            while (recvCount >= (packetSize = BitConverter.ToInt16(RecvPacket.RecvBuffer, 0)))
            {
                var packetObj = ObjectPool<ObjectComponentPacket>.GetInstance.Get();
                packetObj.ResultCode = EResultCode.RESULT_OK;
                packetObj.PacketId = (PACKETID)BitConverter.ToInt16(RecvPacket.RecvBuffer, 2);
                PacketToBytes.SplitPacketBuffer(recvCount, RecvPacket, packetObj);


                await EnqueueResult(packetObj);

                IncreasePacket(packetObj.PacketId);
                recvCount -= packetSize;
            }

            RecvPacket.SaveRemainBuffer(recvCount);
            
            return true;
        }
    }
}

