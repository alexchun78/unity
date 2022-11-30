using System;
using System.Collections.Generic;
using System.Text;
// 이렇게 Jobqueue 관리를 각각 하나씩 만들어서 하는 경우를 보여줌.
// 여기서는 실제로는 사용 안함
namespace Server
{
    interface ITask
    {
        void Excute();
    }

    class BroadcastTask : ITask
    {
        GameRoom _room;
        ClientSession _session;
        string _chat;

        BroadcastTask(GameRoom room, ClientSession session, string chat)
        {
            _room = room;
            _session = session;
            _chat = chat;
        }

        public void Excute()
        {
            _room.Broadcast(_session, _chat);
        }
    }

    class TaskQueue
    {
        Queue<ITask> _queue = new Queue<ITask>();
    }
}
