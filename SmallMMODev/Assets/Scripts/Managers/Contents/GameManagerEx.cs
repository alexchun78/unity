using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    // int  <--> GameObject  
    GameObject _player;
    // ���� ��Һ��� ���� �����ϴ� �� ����. 
    //Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();

    // ������ �ٱ� �������� ID ������ �ʿ䰡 �����Ƿ�, Dictionary ���ٴ� HashSet���� �����Ѵ�.
    //Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public Action<int> OnSpawnEvent;

    public GameObject GetPlayer()
    {
        return _player;
    }

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject obj = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(obj);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Player:
                _player = obj;
                break;
        }
        return obj;
    }

    public Define.WorldObject GetWorldObjectType(GameObject obj)
    {
        BaseController bc = obj.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.UnKnown;

        return bc.WorldObjectType;
    }

    public void DeSpawn(GameObject obj)
    {
        Define.WorldObject type = GetWorldObjectType(obj); 
        switch(type)
        {
            case Define.WorldObject.Monster:
                if(_monsters.Contains(obj))
                {
                    _monsters.Remove(obj);
                    if (OnSpawnEvent != null)
                        OnSpawnEvent.Invoke(-1);
                }
                break;
            case Define.WorldObject.Player:
                {
                    if (_player == obj)
                        _player = null;
                }
                break;
        }
        Managers.Resource.Destory(obj);
    }
}
