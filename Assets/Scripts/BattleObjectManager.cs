﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleObjectTypeComparer : IEqualityComparer<BattleObjectType>
{
    public bool Equals(BattleObjectType t1, BattleObjectType t2)
    {
        return t1 == t2;
    }

    public int GetHashCode(BattleObjectType t)
    {
        return (int)t;
    }
}

public class BattleObjectManager
{
    Dictionary<BattleObjectType, List<BattleObject>> objListDict;

    public void Init()
    {
        objListDict = new Dictionary<BattleObjectType, List<BattleObject>>(new BattleObjectTypeComparer());
    }

    public void AddObject(BattleObject obj)
    {
        BattleObjectType objType = obj.objectType;
        List<BattleObject> objList;
        if (!objListDict.TryGetValue(objType, out objList))
        {
            objList = new List<BattleObject>();
            objListDict.Add(objType, objList);
        }
        objList.Add(obj);
    }

    public void RemoveObject(BattleObject obj)
    {
        BattleObjectType objType = obj.objectType;
        List<BattleObject> objList;
        if (objListDict.TryGetValue(objType, out objList))
            objList.Remove(obj);
    }

    public List<BattleObject> GetObjectList(BattleObjectType objType)
    {
        return objListDict[objType];
    }

    public void Update()
    {
        CheckCollisionPair(BattleObjectType.Player, BattleObjectType.EnemyBullet);
        CheckCollisionPair(BattleObjectType.Player, BattleObjectType.Item);
        CheckCollisionPair(BattleObjectType.Enemy, BattleObjectType.PlayerBullet);
    }

    private void CheckCollisionPair(BattleObjectType type1, BattleObjectType type2)
    {
        List<BattleObject> objList1, objList2;
        if (objListDict.TryGetValue(type1, out objList1) && objListDict.TryGetValue(type2, out objList2))
        {
            foreach (BattleObject obj1 in objList1)
            {
                foreach (BattleObject obj2 in objList2)
                {
                    if (obj1.valid && obj2.valid && obj1.CheckCollision(obj2))
                    {
                        obj1.OnCollision(obj2);
                        obj2.OnCollision(obj1);
                    }
                }
            }
        }
    }
}
