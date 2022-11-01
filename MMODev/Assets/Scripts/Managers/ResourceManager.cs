using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        // 1. original �̹� ��� �ִ� �ְ� �ִ���?
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Debug.Log($"Fail to load prefab : {path}");
            return null;
        }

        // 2. Ȥ�� Ǯ���� �ְ� �ִ���?
        GameObject obj =  Object.Instantiate(prefab, parent);
        obj.name = prefab.name;
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

        Object.Destroy(obj, time);
    }

}
