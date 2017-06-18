using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStageManager : MonoBehaviour
{
    private static BattleStageManager instance;
    public static BattleStageManager Instance {
        protected set {
            if (instance != null)
                Destroy(instance.gameObject);
            instance = value;
        }
        get { return instance; }
    }

    private GameResourceManager resourceManager;
    private BattleObjectManager battleManager;
    private BattleScriptManager scriptManager;

    void Awake()
    {
        Instance = this;
        resourceManager = new GameResourceManager();
        resourceManager.Init();
        battleManager = new BattleObjectManager();
        battleManager.Init();
        scriptManager = new BattleScriptManager();
        scriptManager.Init();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(960, 720, false);

        InitPlayer();
    }

    private void InitPlayer()
    {
        GameObject playerObj = Instantiate(Resources.Load<GameObject>("Player/Player"));
        GameObject subWeaponObj = Instantiate(Resources.Load<GameObject>("Player/SubWeapon"));
        playerObj.transform.SetParent(transform);
        subWeaponObj.transform.SetParent(transform);

        SubWeapon subWeapon = subWeaponObj.GetComponent<SubWeapon>();
        playerObj.GetComponent<PlayerController>().subWeapon = subWeapon;
        subWeapon.playerTrans = playerObj.transform;
    }

    void Update()
    {
        battleManager.Update();
        scriptManager.Update();
    }

    void OnDestroy()
    {
        scriptManager.Dispose();
    }

    public GameObject SpawnObject(string name)
    {
        return resourceManager.Spawn(name);
    }

    public void DespawnObject(GameObject obj)
    {
        resourceManager.Despawn(obj);
    }

    public BattleObjectManager GetBattleManager()
    {
        return battleManager;
    }

    public void AddBattleObject(BattleObject obj)
    {
        battleManager.AddObject(obj);
    }

    public void RemoveBattleObject(BattleObject obj)
    {
        battleManager.RemoveObject(obj);
    }
}
