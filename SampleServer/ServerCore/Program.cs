using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    // 메모리 배리어
    // (A) 코드 재배치 방지
    // (B) 가시성
    // 1) Full Memory Barrior  (ASM MFENCE) : Store / Load 둘다 막는다.  -> Thread.MemoryBarrier();
    // 2) Store Memory Barrior (ASM SFENCE) : Store만 막는다.
    // 2) Load Memory Barrior (ASM LFENCE) : Load만 막는다.
    public partial class Program
    {
        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static void Thread_1()
        {
            y = 1; // Store y

            //-------------------------------
            Thread.MemoryBarrier();

            r1 = x; // Load x
        }

        static void Thread_2()
        {
            x = 1; // Store x

            //-------------------------------
            Thread.MemoryBarrier();

            r2 = y; // Load y
        }

#if false
        int _answer;
        bool _complete;
        void A()
        {
            _answer = 123;
            Thread.MemoryBarrier(); // 기록 후
            _complete = true;
            Thread.MemoryBarrier();// 기록 후
        }

        void B()
        {
            Thread.MemoryBarrier();// 읽기 전
            if (_complete)
            {
                Thread.MemoryBarrier(); // 읽기 전
                Console.WriteLine(_answer);
            }
        }
#endif
        //static void Main(string[] args)
        //{
        //    int count = 0;
        //    while (true)
        //    {
        //        count++;
        //        x = y = r1 = r2 = 0;

        //        Task t1 = new Task(Thread_1);
        //        Task t2 = new Task(Thread_2);
        //        t1.Start();
        //        t2.Start();

        //        Task.WaitAll(t1, t2);

        //        if (r1 == 0 && r2 == 0)
        //            break;
        //    }
        //    Console.WriteLine($"{count}번만에 탈출!");
        //}
    }
}