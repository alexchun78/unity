using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
#if true

    public partial class Program
    {
        static int _num02 = 0;
        static Mutex _lock02 = new Mutex();
        // 커널 단에서 제공을 한다.
        // 잠그고 푼 카운트를 가지고 있고,
        // 쓰레드 아이디도 가지고 있다. 
        // 그래서, 비용이 크다.
        // 대부분의 경우는 AutoResetEvent 까지만 사용하고, Mutex가 필요한 경우는 적다.

        static void ThreadA()
        {
            for (int i = 0; i < 100000; ++i)
            {
                _lock02.WaitOne(); //  소유권 획득
                _num02++;
                _lock02.ReleaseMutex();// 소유권 상실 
            }
        }

        static void ThreadB()
        {
            for (int i = 0; i < 100000; ++i)
            {
            _lock02.WaitOne(); //  소유권 획득
            _num02--;
            _lock02.ReleaseMutex();// 소유권 상실 
            }

        }

        //static void Main(string[] args)
        //{
        //    Task task1 = new Task(ThreadA);
        //    Task task2 = new Task(ThreadB);
        //    task1.Start();
        //    task2.Start();

        //    Task.WaitAll(task1, task2);

        //    Console.WriteLine(_num02);
        //}
    }
#endif
    }
