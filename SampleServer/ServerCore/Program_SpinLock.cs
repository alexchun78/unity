using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // 내가 원하는 것을 얻을 때까지 계속 돌면서 기다린다.
    class SpinLock
    {        
        volatile int _locked = 0; // 0 이면 열려 있고, 1 이면 잠긴 거다 
        
        public void Acquire()
        {
            while (true) {
                //// original은 값이 변경되기 전
                //int original = Interlocked.Exchange(ref _locked, 1);
                //if (original == 0)
                //    return;

                // CAS : Compare and Swap
                int expected = 0;
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    return;                 
            }
        }

        public void Release()
        {
            _locked = 0;
        }

    }

    public partial class Program
    {
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void Thread1()
        {
            for (int i = 0; i < 1000000; ++i)
            {
                _lock.Acquire(); //  소유권 획득
                _num++;
                _lock.Release();// 소유권 상실 
            }
        }

        static void Thread2()
        {
            for (int i = 0; i < 1000000; ++i)
            {
                _lock.Acquire(); //  소유권 획득
                _num--;
                _lock.Release();// 소유권 상실 
            }

        }

        static void Main(string[] args)
        {
            Task task1 = new Task(Thread1);
            Task task2 = new Task(Thread2);
            task1.Start();
            task2.Start();

            Task.WaitAll(task1, task2);

            Console.WriteLine(_num);
        }
    }
}
