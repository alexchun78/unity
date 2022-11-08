using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {
        volatile static bool _stop = false;

        static void ThreadMain()
        {
            Console.WriteLine("Thread Start");
            while(_stop == false)
            {
                // wait for stop signal
            }
            Console.WriteLine("Thread End");
        }

        static void Main(string[] args)
        {
            Task task = new Task(ThreadMain);
            task.Start();

            Thread.Sleep(1000);

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");
            task.Wait();
            Console.WriteLine("종료 성공");
        }
    }
}
