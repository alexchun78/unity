using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Stat
namespace Data
{
    [Serializable]
    public class Stat
    {
        public int level;
        public int hp;
        public int attack;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> _statDict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)
            {
                _statDict.Add(stat.level, stat);
            }
            return _statDict;
        }
    }
}
#endregion