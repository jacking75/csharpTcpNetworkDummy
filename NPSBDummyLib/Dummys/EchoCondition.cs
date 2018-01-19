using System;
using System.Collections.Generic;
using System.Text;

namespace NPSBDummyLib.Dummys
{
    public class EchoCondition
    {
        public string IP;
        public int Port;
        public int PacketSizeMin;
        public int PacketSizeMax;

        DateTime EchoTime;
        int EchoCount;

        public void Set(int echoCount, int echoTiimeSecond)
        {
            EchoCount = echoCount;
            EchoTime = DateTime.Now.AddSeconds(echoTiimeSecond);
        }

        public bool IsEnd(int curEchoCount)
        {
            if (EchoCount > 0)
            {
                if (EchoCount == curEchoCount)
                {
                    return true;
                }
            }
            else
            {
                if (EchoTime <= DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
