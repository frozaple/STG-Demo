using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleObjectManager
{
    List<BattleObject>[] objListArray;

    public void Init()
    {
        objListArray = new List<BattleObject>[(int)BattleObjectType.Max - 1];
    }

    public void AddObject(BattleObject obj)
    {
        BattleObjectType objType = obj.objectType;
        List<BattleObject> objList = objListArray[(int)objType - 1];
        if (objList == null)
        {
            objList = new List<BattleObject>();
            objListArray[(int)objType - 1] = objList;
        }
        objList.Add(obj);
    }

    public void RemoveObject(BattleObject obj)
    {
        BattleObjectType objType = obj.objectType;
        List<BattleObject> objList = objListArray[(int)objType - 1];
        if (objList != null)
        {
            objList.Remove(obj);
        }
    }

    public List<BattleObject> GetObjectList(BattleObjectType objType)
    {
        return objListArray[(int)objType - 1];
    }

    public void Update()
    {
        CheckCollisionPair(BattleObjectType.Player, BattleObjectType.EnemyBullet);
        CheckCollisionPair(BattleObjectType.Enemy, BattleObjectType.PlayerBullet);
    }

    private void CheckCollisionPair(BattleObjectType type1, BattleObjectType type2)
    {
        List<BattleObject> objList1 = objListArray[(int)type1 - 1];
        List<BattleObject> objList2 = objListArray[(int)type2 - 1];
        if (objList1 != null && objList2 != null)
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
