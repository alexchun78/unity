using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField]
    protected int _gold;
    [SerializeField]
    protected int _exp;

    public int Gold { get { return _gold; } set { _gold = value; } }
    public int Exp { get { return _exp; } set { _exp = value; } }

    private void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _defense = 5;
        _moveSpeed = 5.0f;
        _gold = 0;
        _exp = 0;
    }
}
