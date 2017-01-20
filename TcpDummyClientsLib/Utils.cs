using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpDummyClientsLib
{
    class Utils
    {
        static public Tuple<int,int> MinMaxThreadCount()
        {
            int minWorkThreads, maxWorkThreads = 0;
            int iocpThreads = 0;

            System.Threading.ThreadPool.GetMaxThreads(out minWorkThreads, out iocpThreads);
            System.Threading.ThreadPool.GetMinThreads(out maxWorkThreads, out iocpThreads);

            return Tuple.Create(minWorkThreads, maxWorkThreads);
        }

        static public Tuple<int, int> 나누기_몫과나머지(int 제수, int 피제수)
        {
            int 몫 = (int)(피제수 / 제수);
            int 나머지 = (int)(피제수 % 제수);
            return Tuple.Create(몫, 나머지);
        }
    }
}
