//#define Coroutine 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
#if Coroutine // Coroutine
    class Test
    {
        public int _id;
    }
    class CoroutineTest : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            for(int i = 0; i < 100000000; ++i)
            {
                if (i % 100000 == 0)
                    yield return null;
            }
    }
    }

    Coroutine co;
#endif
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        // UI
        Managers.UIManager.ShowSceneUI<UI_Inven>();

        Dictionary<int, Data.Stat> test = Managers.Data.StatDict;
#if Coroutine // Coroutine
        CoroutineTest test = new CoroutineTest();
        foreach(System.Object t in test)
        {
            Debug.Log(t);
        }
        // 유니티에서 사용하는 Coroutine : 특정 시점에서 함수 실행을 멈추었다가 내가 원하는 시점에 다시 시작하게 할 수 있다.
        co = StartCoroutine("CoExplodeAfterSeconds", 4.0f);
        StartCoroutine("CoStopExplodeAfterSeconds", 2.0f);
#endif
    }

    public override void Clear()
    {
    }

#if Coroutine // Coroutine
    IEnumerator CoStopExplodeAfterSeconds(float second)
    {
        Debug.Log("Stop Enter");
        yield return new WaitForSeconds(second);
        Debug.Log("Stop Coroutine!!");
        if(co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    IEnumerator CoExplodeAfterSeconds(float second)
    {
        Debug.Log("Explode Enter");
        yield return new WaitForSeconds(second);
        Debug.Log("Explode Boom!!");
        co = null;
    }
#endif
}
