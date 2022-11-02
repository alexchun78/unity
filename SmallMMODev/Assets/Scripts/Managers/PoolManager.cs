using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager 
{
    #region Pool
    class Pool
    {
        public Transform Root { get; set; }
        public GameObject Original { get; private set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{Original.name}_Root";

            for(int i = 0; i < count;++i)
            {
                Push(Create());
            }
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>(); // 해당 Object에 Poolable 컴포넌트가 있으면 가져오고, 없으면 추가해서 가져온다.
                                                     // 즉, 무조건 해당 Object에 컴포넌트를 있게 된다.
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent = null)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            poolable.gameObject.SetActive(true);

            // [NOTE] DontDestroryOnLoad 해제 용도
            if(parent == null)
            {
                poolable.transform.parent = Managers.SceneEX.CurrentScene.transform;
            }

            poolable.transform.parent = parent;
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();

    Transform _root;

    public void Init()
    {
        if(_root == null)
        {
            _root = new GameObject { name = "@Root_Pool" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }
     
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if(_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }


        _pool[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        string str = original.name;
        if (_pool.ContainsKey(str) == false)
        {
            CreatePool(original);
        }
               
        return _pool[str].Pop(parent);
    }

    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original,count);
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);


        return;
    }

    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
        {
            return null;
        }
        return _pool[name].Original;

    }

    public void Clear()
    {
        // 해당 transform 산하의 모든 객체 날려버리기
        foreach(Transform child in _root)
        {
            GameObject.Destroy(child.gameObject);
        }
        _pool.Clear();
    }
}
