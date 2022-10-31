using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    // Unity의 모든 오브젝트들은 UnityEngine.Object 로 저장 가능하다. !! 
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        // enum 값들을 불러와서 string으로 변환, 리플렉션 기능
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objs = new UnityEngine.Object[names.Length];

        _objects.Add(typeof(T), objs);

        for (int i = 0; i < names.Length; ++i)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objs[i] = Util.FindChild(gameObject, names[i], true);
            }
            else
            {
                objs[i] = Util.FindChild<T>(gameObject, names[i], true);
            }

            if (objs[i] == null)
                Debug.Log($"failed to Bind!! {names[i]}");
        }
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
        {
            return null;
        }

        return objects[idx] as T;
    }

    protected Text GetText(int idx)
    {
        return Get<Text>(idx);
    }

    protected Button GetButton(int idx)
    {
        return Get<Button>(idx);
    }

    protected Image GetImage(int idx)
    {
        return Get<Image>(idx);
    }

    public static void AddUIEvent(GameObject obj, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        var evt = Util.GetOrAddComponent<UI_EventHandler>(obj);
        
        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }
}
 