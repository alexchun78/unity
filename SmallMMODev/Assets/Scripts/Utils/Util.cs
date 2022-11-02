using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }
    public static GameObject FindChild(GameObject obj, string name = null, bool isRecursive = false)
    {
        Transform trans = FindChild<Transform>(obj, name, isRecursive);
        if (trans == null)
            return null;
        return trans.gameObject;
    }
    public static T FindChild<T>(GameObject obj, string name = null, bool isRecursive = false) where T : UnityEngine.Object
    {
        if (obj == null)
            return null;

        if(isRecursive == false)
        {
            for(int i = 0; i < obj.transform.childCount; ++i)
            {
                Transform trans = obj.transform.GetChild(i);
                if(string.IsNullOrEmpty(name) || trans.name == name)
                {
                    T component = trans.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {

            foreach(T component in obj.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                {
                    return component;
                }

            }
        }
        return null;
    }
}
