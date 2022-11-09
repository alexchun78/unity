using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public partial class Program
    {
        static volatile int countRW = 0;
        static RWLock _lockRW = new RWLock();
        /*
        static void Main(string[] args)
        {
            Task t1 = new Task(delegate ()
            {
                for(int i =0; i < 100000; ++i)
                {
                    _lockRW.WriteLock();
                    countRW++;
                    _lockRW.WriteUnLock();
                }
            });
            Task t2 = new Task(delegate ()
            {
                for (int i = 0; i < 100000; ++i)
                {
                    _lockRW.WriteLock();
                    countRW--;
                    _lockRW.WriteUnLock();
                }
            });

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(countRW);
        }
        */
    }
}
