/*
#define TLS
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public partial class Program
    {
#if TLS
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(()=> { return $"My name is {Thread.CurrentThread.ManagedThreadId}"; });
#else
        static string ThreadName;
#endif

        static void WhoAmI()
        {
#if TLS
            //ThreadName.Value = $"My name is {Thread.CurrentThread.ManagedThreadId}";
            bool repeat = ThreadName.IsValueCreated;
            if(repeat == true)
            {
                Console.WriteLine(ThreadName.Value + " (repeat)");
            }
            else
            {
                Console.WriteLine(ThreadName.Value);
            }
#else
            ThreadName = $"My name is {Thread.CurrentThread.ManagedThreadId}";
#endif

           // Thread.Sleep(1000);

#if TLS
            //Console.WriteLine(ThreadName.Value);
#else
            Console.WriteLine(ThreadName);
#endif
        }

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMinThreads(2,2);
            // 이렇게 하면, 내부적으로 Task를 개수 만큼 만들어서 제공한다.
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);

            // TLS 해제하기
            ThreadName.Dispose();
        }
    }
}
*/