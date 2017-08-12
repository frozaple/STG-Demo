using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStageManager : MonoBehaviour
{
    static private AnimationCurve deathEffectScaleCurve = new AnimationCurve(new Keyframe[] {
        new Keyframe(0, 0, 0, 200),
        new Keyframe(0.25f, 50, 200, 200),
        new Keyframe(0.75f, 400, 700, 0),
        new Keyframe(1f, 400, 0, 0),
    });
    static private AnimationCurve deathEffectScaleCurveEx = new AnimationCurve(new Keyframe[] {
        new Keyframe(0, 0, 0, 300),
        new Keyframe(0.25f, 75, 300, 300),
        new Keyframe(0.75f, 425, 700, 0),
        new Keyframe(1f, 425, 0, 0),
    });

    static private BattleStageManager instance;
    static public BattleStageManager Instance {
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
    private CameraShake camShakeEffect;

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
        camShakeEffect = Camera.main.GetComponent<CameraShake>();
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

    // -------------------------- resource manager --------------------------

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

    // -------------------------- battle manager --------------------------

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

    public void RangeEnemyDamage(Vector3 centerPos, float range, int damage)
    {
        battleManager.RangeEnemyDamage(centerPos, range, damage);
    }

    public void RangeBulletEliminate(Vector3 centerPos, float range)
    {
        battleManager.RangeBulletEliminate(centerPos, range);
    }

    public void AddRangeTask(RangeTask task)
    {
        battleManager.AddRangeTask(task);
    }

    // -------------------------- player manager --------------------------

    public PlayerStateManager GetPlayerManager()
    {
        return playerManager;
    }

    public Vector3 GetPlayerPos()
    {
        return playerManager.GetPlayerPos();
    }

    public float GetPlayerAngle(float posX, float posY)
    {
        return playerManager.GetPlayerAngle(posX, posY);
    }

    // -------------------------- random --------------------------

    public int GetRandom(int min, int max)
    {
        return battleRandom.Next(min, max);
    }

    // -------------------------- effects --------------------------

    public void PlayDeathEffect(Vector3 pos)
    {
        for (int i = 0; i < 2; i++)
            SpawnDeathEffect(pos + new Vector3(0, i * 64f - 32f), 80f, 0, deathEffectScaleCurve);
        for (int i = 0; i < 2; i++)
            SpawnDeathEffect(pos + new Vector3(i * 64f - 32f, 0), 80f, 0, deathEffectScaleCurve);
        SpawnDeathEffect(pos, 80f, 0, deathEffectScaleCurveEx);
        SpawnDeathEffect(pos, 50f, 30f, null);
    }

    private void SpawnDeathEffect(Vector3 pos, float duration, float delay, AnimationCurve curve)
    {
        GameObject deathEff = BattleStageManager.Instance.SpawnObject("Player/PlayerDeathEffect");
        deathEff.transform.position = pos;
        EffectObject effObj = deathEff.GetComponent<EffectObject>();
        effObj.lifeDuration = duration;
        effObj.SetDelay(delay);
        effObj.SetScaleCurve(curve);
    }

    public void PlayCameraShake(float duration, float x, float y)
    {
        camShakeEffect.DoShake(duration, x, y);
    }
}
