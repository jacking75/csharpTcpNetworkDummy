using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NPSBDummyLib.TestCase
{
    //TODO: 삭제 예정
    /*
    public class ManyConnect
    {
        Int64 ConnectedCount = 0;
        List<Dummy.Dummy> DummyList = new List<Dummy.Dummy>();

        public string Prepare(int dummyCount)
        {
            for (int i = 0; i < dummyCount; ++i)
            {
                DummyList.Add(new Dummy.Dummy());
            }

            return "";
        }
        public async Task<string> ProcessAsync(string ip, int port)
        {
            ConnectedCount = 0;

            //var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                for (int i = 0; i < DummyList.Count; ++i)
                {
                    await Task.Run(async () => await DummyList[i].ConnectAsync(ip, port, 10));

                    System.Threading.Interlocked.Increment(ref ConnectedCount);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return $"[Success] 접속 완료: {ConnectedCount}";
        }

        public async Task<string> EndAsync()
        {
            for (int i = 0; i < DummyList.Count; ++i)
            {
                await Task.Run(async () => await DummyList[i].EndAsync());

                System.Threading.Interlocked.Decrement(ref ConnectedCount);
            }

            return $"[Success] 현재 접속 중인 수: {ConnectedCount}";
        }

        public Int64 CurrentConnectedCount()
        {
            return System.Threading.Interlocked.Read(ref ConnectedCount);
        }
    }
    */
}
