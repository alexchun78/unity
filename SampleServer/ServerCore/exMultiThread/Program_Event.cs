/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
#if false // https://www.csharpstudy.com/Threads/autoresetevent.aspx
    public partial class Program
    {
        // AutoResetEvent 객체 필드
        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        static void Main()
        {
            // 쓰레드 A 생성
            Thread A = new Thread(Run);
            A.Name = "Thread A";
            A.Start();

            // 메인쓰레드            
            Thread.Sleep(3000); //3초 대기
            autoEvent.Set(); // 쓰레드 A에 신호
        }

        static void Run()
        {
            string name = Thread.CurrentThread.Name;
            Console.WriteLine("{0}: Run Start", name);

            // AutoResetEvent 신호 대기
            autoEvent.WaitOne();
            Console.WriteLine("{0} : DoWork", name);

            Console.WriteLine("{0}: Run End", name);
        }
    }
#endif
#if true
    class Lock
    {
        // bool <- 커널
        // true : 문이 열린 상태
        // false : 문이 닫힌 상태
        // 엄청 비용이 많다.
#if true // AutoResetEvent
        AutoResetEvent _available = new AutoResetEvent(true);
#else
        ManualResetEvent _available = new ManualResetEvent(true);
#endif

        public void Acquire()
        {
            _available.WaitOne(); // 입장 시도 Auto일 경우, _available.Reset()이 포함된다.
       //     _available.Reset(); // bool -> false // 문을 닫는다.
        }

        public void Release()
        {
            _available.Set(); // 문을 열어준다.
        }
    }

    public partial class Program
    {
        static int _num01 = 0;
        static Lock _lock01 = new Lock();

        static void Thread101()
        {
            for (int i = 0; i < 1000; ++i)
            {
                _lock01.Acquire(); //  소유권 획득
                _num01++;
                _lock01.Release();// 소유권 상실 
            }
        }

        static void Thread201()
        {
            for (int i = 0; i < 1000; ++i)
            {
                _lock01.Acquire(); // 소유권 획득
                _num01--;
                _lock01.Release();// 소유권 상실 
            }

        }

        //static void Main(string[] args)
        //{
        //    Task task1 = new Task(Thread101);
        //    Task task2 = new Task(Thread201);
        //    task1.Start();
        //    task2.Start();

        //    Task.WaitAll(task1, task2);

        //    Console.WriteLine(_num01);
        //}
    }
#endif
    }
*/
