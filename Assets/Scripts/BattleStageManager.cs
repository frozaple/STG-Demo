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
    private SpriteManager spriteManager;
    private PlayerStateManager playerManager;

    private System.Random battleRandom;

    void Awake()
    {
        Instance = this;
        resourceManager = new GameResourceManager();
        resourceManager.Init();
        battleManager = new BattleObjectManager();
        battleManager.Init();
        scriptManager = new BattleScriptManager();
        scriptManager.Init();
        spriteManager = new SpriteManager();
        spriteManager.InitSprites();
        playerManager = new PlayerStateManager();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(960, 720, false);

        playerManager.InitPlayer();
        battleRandom = new System.Random();
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

    public void DespawnObject(GameObject obj, bool scriptDespawn = false)
    {
        if (scriptDespawn)
            scriptManager.Despawn(obj);
        else
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

    public Sprite GetBulletSprite(int shape, int color)
    {
        return spriteManager.GetBulletSprite(shape, color);
    }

    public PlayerStateManager GetPlayerManager()
    {
        return playerManager;
    }

    public float GetPlayerAngle(float posX, float posY)
    {
        return playerManager.GetPlayerAngle(posX, posY);
    }

    public int GetRandom(int min, int max)
    {
        return battleRandom.Next(min, max);
    }
}
