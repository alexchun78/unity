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
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Debug.Log($"Fail to load prefab : {path}");
            return null;
        }
        return Object.Instantiate(prefab);
    }

    public void Destory(GameObject obj, float time = 0.0f)
    {
        if (obj == null)
            return;
        Object.Destroy(obj, time);
    }

}
