using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager 
{
    Transform _root;

    public void Init()
    {
        if(_root == null)
        {
            _root = new GameObject { name = "@Root_Pool" }.transform;
        }
    }

}
