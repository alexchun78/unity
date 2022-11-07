using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            string name = path;
            int idx = name.LastIndexOf('/');
            if(idx > 0)
                name = name.Substring(idx + 1);

            GameObject obj = Managers.Pool.GetOriginal(name);
            if (obj != null)
                return obj as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        // 1. original �̹� ��� �ִ� �ְ� �ִ���?
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if(original == null)
        {
            Debug.Log($"Fail to load prefab : {path}");
            return null;
        }

        // 2. Ȥ�� Ǯ���� �ְ� �ִ���?
        if(original.GetComponent<Poolable>() != null)
        {
            return Managers.Pool.Pop(original, parent).gameObject;
        }

        GameObject obj =  Object.Instantiate(original, parent);
        obj.name = original.name;
        //int idx = obj.name.IndexOf("(Clone)");
        //if(idx > 0)
        //{
        //    string sub = obj.name.Substring(0, idx);
        //    obj.name = sub;
        //}

        return obj;
    }

    public void Destory(GameObject obj, float time = 0.0f)
    {
        if (obj == null)
            return;

        // ���� Ǯ���� �ʿ��� �ֶ��, Ǯ�� �Ŵ������� ��Ź�Ѵ�.
        Poolable poolable = obj.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(obj, time);
    }

}
