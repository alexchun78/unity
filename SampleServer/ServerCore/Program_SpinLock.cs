using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
#if true // C#에서 제공하는 SpinLock 사용

    // 1. 근성
    // 2. 양보
    // 3. 갑질

    // 상호 배제

    public partial class Program
    {
        static object _obj = new object();
        // 기본적으로 계속 기다리다가, 너무 답이 없으면, 2.양보 모드로 변환되어 다른 쓰레드에 넘겨준다. 
        static SpinLock _lock = new SpinLock(); 

        static void Main(string[] args)
        {
            //Monitor.Enter(_obj);
            //Monitor.Exit(_obj);
            lock (_obj)
            {

            }

            bool lockTaken = false;
            try
            {

                _lock.Enter(ref lockTaken);
            }
            finally
            {
                if(lockTaken == true)
                    _lock.Exit();
            }
        }
    }

#else // SpinLock 직접 구현

    // 내가 원하는 것을 얻을 때까지 계속 돌면서 기다린다.
    class SpinLock
    {        
        volatile int _locked = 0; // 0 이면 열려 있고, 1 이면 잠긴 거다 
        
        public void Acquire()
        {
            while (true) {
                // [CASE #1] CAS : Compare and Swap
                int expected = 0;
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                // [CASE #2] 쉬다 오는 경우
                //Thread.Sleep(1); // 무조건 휴식, 1ms 정도 쉰다.
                //Thread.Sleep(0); // 조건부 양보 -> 나보다 우선순위가 낮은 애들한테는 양보 불가 
                                // 우선순위가 높거나 같은 애들이 없으면 나한테 다시 온다.
                Thread.Yield(); // 관대한 양보 -> 지금 실행 가능한 쓰레드가 있으면 실행한다.
                                // 실행 가능한 쓰레드가 없으면 나한테 다시 온다. 

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

        //static void Main(string[] args)
        //{
        //    Task task1 = new Task(Thread1);
        //    Task task2 = new Task(Thread2);
        //    task1.Start();
        //    task2.Start();

        //    Task.WaitAll(task1, task2);

        //    Console.WriteLine(_num);
        //}
    }
#endif

}
