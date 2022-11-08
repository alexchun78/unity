using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    class Program
    {

        static void Main(string[] args)
        {
            int[,] arr = new int[10000, 10000];

            {
                var now = DateTime.Now.Ticks;
                for(int c = 0; c < 10000; ++ c)
                {
                    for(int r = 0; r < 10000; ++r)
                    {
                        arr[c, r] = 1;
                    }
                }
                var end = DateTime.Now.Ticks;
                Console.WriteLine($"arr[c, r] 순서 걸린 시간 : {end - now}");
            }
            {
                var now = DateTime.Now.Ticks;
                for (int c = 0; c < 10000; ++c)
                {
                    for (int r = 0; r < 10000; ++r)
                    {
                        arr[r, c] = 1;
                    }
                }
                var end = DateTime.Now.Ticks;
                Console.WriteLine($"arr[r, c] 순서 걸린 시간 : {end - now}");
            }

        }
    }
}
