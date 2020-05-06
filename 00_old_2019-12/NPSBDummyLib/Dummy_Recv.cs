﻿using CSBaseLib;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;

namespace NPSBDummyLib
{
    public partial class Dummy
    {
        Task WorkerThread;
        Dictionary<PACKETID, int> PacketStatDic = new Dictionary<PACKETID, int>();
        Channel<TaskCompletionSource<(EResultCode, PACKETID, byte[])>> RecvResultChannel = Channel.CreateUnbounded<TaskCompletionSource<(EResultCode, PACKETID, byte[])>>();
        //ConcurrentQueue<(EResultCode, PACKETID, byte[])> RecvResultChannel = new ConcurrentQueue<(EResultCode, PACKETID, byte[])>();


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
        
        public async Task EnqueueResult((EResultCode, PACKETID, byte[]) result)
        {
            var tcs = new TaskCompletionSource<(EResultCode, PACKETID, byte[])>(TaskCreationOptions.RunContinuationsAsynchronously);
            tcs.SetResult(result);
            await RecvResultChannel.Writer.WriteAsync(tcs);
            await tcs.Task;
        }

        public async Task<(EResultCode, PACKETID, byte[])> PopRecvResult(int limitActionTime)
        {
            var cts = new CancellationTokenSource(limitActionTime);
            await RecvResultChannel.Reader.WaitToReadAsync(cts.Token);

            if (!RecvResultChannel.Reader.TryRead(out var resultTask))
            {
                return (EResultCode.RESULT_FAILED_POP_CHANNEL, 0, null);
            }

            return resultTask.Task.Result;
        }
        
/*
        public void EnqueueResult((EResultCode, PACKETID, byte[]) result)
        {
            RecvResultChannel.Enqueue(result);
            
        }

        public async Task<(EResultCode, PACKETID, byte[])> PopRecvResult(int limitActionTime, ActionBase action)
        {
            DateTime prevTime = DateTime.Now;
            while (true)
            {
                if (RecvResultChannel.TryDequeue(out var result))
                {
                    return result;
                }

                await Task.Delay(100);

                var elapsedTime = DateTime.Now - prevTime;
                if (elapsedTime.TotalMilliseconds > limitActionTime)
                {
                    break;
                }
            }

            return (EResultCode.RESULT_FAILED_POP_CHANNEL, 0, null);
        }
*/

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
                        break;
                    }
                }
            });
        }

        public async Task<bool> RecvProc(int recvCount, string recvError)
        {
            if (recvError != "")
            {
                if (DummyManager.IsInProgress())
                {
                    await EnqueueResult((EResultCode.RESULT_RECV_ERROR, 0, null));
                    return false;
                }
                else
                {
                    await EnqueueResult((EResultCode.RESULT_OK, 0, null));
                    return false;
                }
            }

            if (recvCount == 0)
            {
                await EnqueueResult((EResultCode.RESULT_CONNECTION_EXPIRED, PACKETID.CS_END, null));
                return false;
            }

            recvCount = RecvPacket.CombineRemainBuffer(recvCount);

            var packetSize = 0;

            while (recvCount >= (packetSize = BitConverter.ToInt16(RecvPacket.RecvBuffer, 0)))
            {
                if (packetSize > DummyManager.GetDummyInfo.PacketSizeMax - PacketDef.PACKET_HEADER_SIZE)
                {
                    await EnqueueResult((EResultCode.RESULT_EXCEED_PACKET_SIZE, 0, null));
                    return false;
                }

                var packetId = (PACKETID)BitConverter.ToInt16(RecvPacket.RecvBuffer, 2);
                var body = PacketToBytes.SplitPacketBuffer(recvCount, RecvPacket);
                await EnqueueResult((EResultCode.RESULT_OK, packetId, body));

                IncreasePacket(packetId);
                recvCount -= packetSize;
            }

            RecvPacket.SaveRemainBuffer(recvCount);
            
            return true;
        }
    }
}
