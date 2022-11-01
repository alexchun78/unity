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
        // 1. original 이미 들고 있는 애가 있는지?
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Debug.Log($"Fail to load prefab : {path}");
            return null;
        }

        // 2. 혹시 풀링된 애가 있는지?
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

        // 만약 풀링이 필요한 애라면, 풀링 매니저에게 위탁한다.

        Object.Destroy(obj, time);
    }

}
