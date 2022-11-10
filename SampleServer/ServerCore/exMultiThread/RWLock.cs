/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    // 스핀락 정책 => 5000번 이후에는 yield로 자동 변경

    class RWLock
    {
#if false // 재귀적 Lock을 허용할지 (NO)
        const int EMPTY_FLAG = 0X00000000;
        const int WRITE_MASK = 0X7FFF0000;
        const int READ_MASK = 0X0000FFFFFF;
        const int MAX_SPIN_COUNT = 5000;

        // int가 32bit 이므로 bit를 나누어서 사용한다.
        // [Unused(1비트)] , [WriteThreadId(15비트)] , [ReadCount(16비트)]
        int _flag = EMPTY_FLAG;

        public void WriteLock()
        {
            // 쓰는 용의 ID 가져오기
            int _desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을때, 경합해서 소유권을 얻는다.
            while(true)
            {
                for(int i =0; i < MAX_SPIN_COUNT;++i)
                {
                    // 시도해서 성공하면 return 
                    // -> atomic 하지 않아서 멀티 쓰레드에서는 안된다.
                    //if (_flag == EMPTY_FLAG)
                    //{
                    //    _flag = _desired;
                    //}
                    if (Interlocked.CompareExchange(ref _flag, _desired, EMPTY_FLAG) == EMPTY_FLAG)
                        return;
                }

                // 실패하면,
                Thread.Yield();
            }
        }
         
        public void WriteUnLock()
        {
            Interlocked.Exchange(ref _flag, EMPTY_FLAG);       
        }

        public void ReadLock()
        {
            // 아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1 늘린다.
            while(true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; ++i)
                {
                    //if((_flag & WRITE_MASK) == 0)
                    //{
                    //    _flag = _flag + 1;
                    //}
                    int expected = (_flag & READ_MASK); // A(0) B(0)  
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) // A(0->1)
                        return;
                }

                Thread.Yield();
            }
        }

        public void ReadUnLock()
        {
            Interlocked.Decrement(ref _flag);
        }
#else  // 재귀적 Lock을 허용할지 (YES)
        // WriteLock -> WriteLock (OK)
        // WriteLock -> ReadLock (OK)
        // ReadLock -> WriteLock (NO)

        const int EMPTY_FLAG = 0X00000000;
        const int WRITE_MASK = 0X7FFF0000;
        const int READ_MASK = 0X0000FFFFFF;
        const int MAX_SPIN_COUNT = 5000;

        // int가 32bit 이므로 bit를 나누어서 사용한다.
        // [Unused(1비트)] , [WriteThreadId(15비트)] , [ReadCount(16비트)]
        int _flag = EMPTY_FLAG;
        int _writeCount = 0;

        public void WriteLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는 지 확인
            int lockThreadID = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId == lockThreadID)
            {
                _writeCount++;
                return;
            }

            // 쓰는 용의 ID 가져오기
            int _desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을때, 경합해서 소유권을 얻는다.
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; ++i)
                {
                    // 시도해서 성공하면 return 
                    // -> atomic 하지 않아서 멀티 쓰레드에서는 안된다.
                    //if (_flag == EMPTY_FLAG)
                    //{
                    //    _flag = _desired;
                    //}
                    if (Interlocked.CompareExchange(ref _flag, _desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                        
                }

                // 실패하면,
                Thread.Yield();
            }
        }

        public void WriteUnLock()
        {
            int lockCount = --_writeCount;
            if(lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            // 동일 쓰레드가 WriteLock을 이미 획득하고 있는 지 확인
            int lockThreadID = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadID)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            // 아무도 WriteLock을 획득하고 있지 않으면, ReadCount를 1 늘린다.
            while (true)
            {
                for (int i = 0; i < MAX_SPIN_COUNT; ++i)
                {
                    /// 의사코드가 이렇게 변경되는 게 이해가 되어야 한다.
                    //if((_flag & WRITE_MASK) == 0)
                    //{
                    //    _flag = _flag + 1;
                    //}
                    int expected = (_flag & READ_MASK); // A(0) B(0)  
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected) // A(0->1)
                        return;
                }

                Thread.Yield();
            }
        }

        public void ReadUnLock()
        {
            Interlocked.Decrement(ref _flag);
        }

#endif
    }
}
*/