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
        Channel<TaskCompletionSource<(EResultCode, PACKETID, byte[])>> RecvResultChannel = Channel.CreateUnbounded<TaskCompletionSource<(EResultCode, PACKETID, byte[])>>();

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
            await RecvResultChannel.Writer.WriteAsync(tcs).ConfigureAwait(false);
            await tcs.Task.ConfigureAwait(false);
        }

        public async Task<(EResultCode, PACKETID, byte[])> PopRecvResult(int limitActionTime)
        {
            CancellationTokenSource cts = new CancellationTokenSource(limitActionTime);
            await RecvResultChannel.Reader.WaitToReadAsync(cts.Token).ConfigureAwait(false);

            TaskCompletionSource<(EResultCode, PACKETID, byte[])> resultTask;
            if (!RecvResultChannel.Reader.TryRead(out resultTask))
            {
                return (EResultCode.RESULT_FAILED_POP_CHANNEL, 0, null);
            }

            return resultTask.Task.Result;
        }
        
        public void CreateRecvWorker()
        {
            WorkerThread = Task.Run(async () => {
                while (DummyManager.IsInProgress())
                {
                    var (recvCount, recvError) = await ClientSocket.ReceiveAsync(RecvPacket.BufferSize, RecvPacket.RecvBuffer);

                    var result = await RecvProc(recvCount, recvError);
                    if (!result)
                    {
                        if (DummyManager.IsInProgress())
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
                if (DummyManager.IsInProgress())
                {
                    await EnqueueResult((EResultCode.RESULT_OK, 0, null));
                    return false;
                }
                else
                {
                    await EnqueueResult((EResultCode.RESULT_RECV_ERROR, 0, null));
                    return false;
                }
            }

            if (recvCount == 0)
            {
                await EnqueueResult((EResultCode.RESULT_CONNECTION_EXPIRED, 0, null));
                return false;
            }

            recvCount = RecvPacket.CombineRemainBuffer(recvCount);

            var packetSize = 0;

            while (recvCount >= (packetSize = BitConverter.ToInt16(RecvPacket.RecvBuffer, 0)))
            {
                var packetID = (PACKETID)BitConverter.ToInt16(RecvPacket.RecvBuffer, 2);
                var recvBuffer = PacketToBytes.SplitPacketBuffer(recvCount, RecvPacket);

                await EnqueueResult((EResultCode.RESULT_OK, packetID, recvBuffer));

                IncreasePacket(packetID);
                recvCount -= packetSize;
            }

            RecvPacket.SaveRemainBuffer(recvCount);
            
            return true;
        }
    }
}

