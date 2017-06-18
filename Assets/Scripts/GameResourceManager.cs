using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    public GameObjectPool(string objName, int initSize = 8)
    {
        poolObject = new GameObject("<" + objName + ">Pool");
        poolName = objName;
        primaryObject = Resources.Load<GameObject>(objName);
        cachedObjects = new Stack<GameObject>(initSize);
    }

    public GameObject poolObject;
    private string poolName;
    private GameObject primaryObject;
    private Stack<GameObject> cachedObjects;

    public GameObject GetObject()
    {
        GameObject obj;
        if (cachedObjects.Count > 0)
        {
            obj = cachedObjects.Pop();
            obj.SetActive(true);
        }
        else
        {
            obj = GameObject.Instantiate(primaryObject, poolObject.transform);
            PoolObject poolObj = obj.GetComponent<PoolObject>();
            if (poolObj == null)
                poolObj = obj.AddComponent<PoolObject>();
            poolObj.poolName = poolName;
        }

        return obj;
    }

    public void AddObject(GameObject obj)
    {
        obj.SetActive(false);
        cachedObjects.Push(obj);
    }
}

public class GameResourceManager
{
    private GameObject objectPoolRoot;
    private Dictionary<string, GameObjectPool> objectPools = new Dictionary<string, GameObjectPool>();

    public void Init()
    {
        objectPoolRoot = new GameObject("ObjectPoolRoot");
        objectPoolRoot.transform.parent = BattleStageManager.Instance.transform;
        objectPoolRoot.transform.localPosition = Vector3.zero;
    }

    public GameObject Spawn(string name)
    {
        GameObjectPool objPool;
        objectPools.TryGetValue(name, out objPool);
        if (objPool == null)
        {
            objPool = new GameObjectPool(name);
            objPool.poolObject.transform.parent = objectPoolRoot.transform;
            objPool.poolObject.transform.localPosition = Vector3.zero;
            objectPools.Add(name, objPool);
        }
        return objPool.GetObject();
    }

    public void Despawn(GameObject obj)
    {
        PoolObject poolObj = obj.GetComponent<PoolObject>();
        if (poolObj == null)
            GameObject.Destroy(obj);
        string name = poolObj.poolName;
        GameObjectPool objPool;
        objectPools.TryGetValue(name, out objPool);
        if (objPool == null)
            GameObject.Destroy(obj);
        objPool.AddObject(obj);
    }
}
