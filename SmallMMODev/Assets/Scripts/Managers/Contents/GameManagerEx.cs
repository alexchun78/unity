using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    // int  <--> GameObject  
    GameObject _player;
    // 게임 요소별로 따로 관리하는 게 낫다. 
    //Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();

    // 서버가 붙기 전까지는 ID 관리할 필요가 없으므로, Dictionary 보다는 HashSet으로 관리한다.
    //Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
    HashSet<GameObject> _monsters = new HashSet<GameObject>();

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject obj = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(obj);
                break;
            case Define.WorldObject.Player:
                _player = obj;
                break;
        }
        return obj;
    }

    public Define.WorldObject GEtWorldObjectType(GameObject obj)
    {
        BaseController bc = obj.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.UnKnown;

        return bc.WorldObjectType;
    }

    public void DeSpawn(GameObject obj)
    {
        Define.WorldObject type = GEtWorldObjectType(obj); 
        switch(type)
        {
            case Define.WorldObject.Monster:
                if(_monsters.Contains(obj))
                {
                    _monsters.Remove(obj);
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
