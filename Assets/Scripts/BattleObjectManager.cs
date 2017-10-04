using System.Collections;
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

public struct RangeTask {
    private float passTime;
    private float startTime;
    private float endTime;
    private float rangeSpeed;
    public Vector3 rangePos;
    public bool getPoint;
    public int enemyDamage;

    public RangeTask(Vector3 pos, float start, float duration, float range, bool point, int damage)
    {
        rangePos = pos;
        passTime = 0;
        startTime = start;
        endTime = start + duration;
        rangeSpeed = range;
        getPoint = point;
        enemyDamage = damage;
    }

    public float Update()
    {
        int oldTimeInt = (int)(passTime);
        passTime += Time.timeScale;
        if (passTime >= startTime)
        {
            int newTimeInt = (int)(passTime);
            if (oldTimeInt != newTimeInt)
                return (newTimeInt - startTime) * rangeSpeed;
        }
        return 0;
    }

    public bool Expired()
    {
        return passTime >= endTime;
    }
}

public class BattleObjectManager
{
    private Dictionary<BattleObjectType, List<BattleObject>> objListDict;
    private List<RangeTask> pendingRangeTask;

    public void Init()
    {
        objListDict = new Dictionary<BattleObjectType, List<BattleObject>>(new BattleObjectTypeComparer());
        pendingRangeTask = new List<RangeTask>();
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
        if (objListDict.ContainsKey(objType))
            return objListDict[objType];
        return null;
    }

    public void Update()
    {
        UpdateRangeTask();
        CollisionCheck();
    }

    private void CollisionCheck()
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

    public void RangeEnemyDamage(Vector3 centerPos, float range, int damage)
    {
        float rangeSq = range * range;
        List<BattleObject> enemyList = GetObjectList(BattleObjectType.Enemy);
        if (enemyList == null)
            return;
        foreach (BattleObject enemyObj in enemyList)
        {
            Vector3 disVec = enemyObj.transform.position - centerPos;
            if (disVec.sqrMagnitude < rangeSq)
            {
                Enemy enemy = enemyObj as Enemy;
                if (enemy != null)
                    enemy.hp -= damage;
            }
        }
    }

    public void RangeBulletEliminate(Vector3 centerPos, float range, bool getPoint)
    {
        float rangeSq = range * range;
        List<BattleObject> bulletList = GetObjectList(BattleObjectType.EnemyBullet);
        if (bulletList == null)
            return;
        foreach (BattleObject bulletObj in bulletList)
        {
            Vector3 disVec = bulletObj.transform.position - centerPos;
            if (disVec.sqrMagnitude < rangeSq)
            {
                EnemyBullet bullet = bulletObj as EnemyBullet;
                if (bullet != null)
                {
                    bullet.Eliminate();
                    if (getPoint)
                    {
                        GameObject pointItem = BattleStageManager.Instance.SpawnObject("Item/PointItem");
                        pointItem.transform.position = bullet.transform.position;
                    }
                }
            }
        }
    }

    public void AddRangeTask(RangeTask task)
    {
        pendingRangeTask.Add(task);
    }

    private void UpdateRangeTask()
    {
        int taskCount = pendingRangeTask.Count;
        for (int i = taskCount - 1; i >= 0; --i)
        {
            RangeTask task = pendingRangeTask[i];
            float range = task.Update();
            if (range > 0)
            {
                RangeBulletEliminate(task.rangePos, range, task.getPoint);
                if (task.enemyDamage > 0)
                    RangeEnemyDamage(task.rangePos, range, task.enemyDamage);
            }
            if (pendingRangeTask[i].Expired())
                pendingRangeTask.RemoveAt(i);
            else
                pendingRangeTask[i] = task;
        }
    }
}
