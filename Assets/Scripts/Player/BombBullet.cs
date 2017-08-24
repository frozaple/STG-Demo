using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBullet : MonoBehaviour
{
    static private AnimationCurve rotateCurve = new AnimationCurve(new Keyframe[] {
        new Keyframe(0, 0, -2160, 0),
        new Keyframe(0.5f, -1080, -2160, -2160),
        new Keyframe(1, -1440, 0, 0),
    });
    static private AnimationCurve radiusCurve = new AnimationCurve(new Keyframe[] {
        new Keyframe(0, 0, 128, 0),
        new Keyframe(1, 128, 0, 0),
    });
    static private AnimationCurve strikeCurve = new AnimationCurve(new Keyframe[] {
        new Keyframe(0, 0, 0, 0),
        new Keyframe(1, 1, 2, 0),
    });
    static private float followTime = 120f;
    static private float rotateTime = 180f;
    static private float strikeDuration = 30f;
    static private float explosionDuration = 30f;

    public int collisionDamage;
    public int explosionDamage;
    public float collisionRadius;
    public float explosionRadius;
    public Transform subSetTrans;

    private Transform playerTrans;
    private Vector3 strikeBeginPos;
    private Vector3 strikeEndPos;
    private float activateTime;
    private float initRotation;
    private float strikeBeginTime;

    private int subCount;
    private SpriteRenderer[] subBullets;
    private Transform[] subPivots;
    private float[] pivotRotSpeed;
    private float[] bulletRotSpeed;

    void Awake()
    {
        playerTrans = BattleStageManager.Instance.GetPlayerManager().GetPlayerTrans();

        subBullets = subSetTrans.GetComponentsInChildren<SpriteRenderer>();
        subCount = subBullets.Length;
        subPivots = new Transform[subCount];
        for (int i = 0; i < subCount; ++i)
            subPivots[i] = subBullets[i].transform.parent;
        
        pivotRotSpeed = new float[subCount];
        bulletRotSpeed = new float[subCount];
    }

    void OnEnable()
    {
        PlayerStateManager playerMgr = BattleStageManager.Instance.GetPlayerManager();
        activateTime = 0;

        transform.position = playerTrans.position;
        subSetTrans.transform.localPosition = Vector3.zero;

        strikeBeginTime = rotateTime + playerMgr.activeBomb * 2f;
        initRotation = playerMgr.activeBomb * 60f;
        transform.eulerAngles = new Vector3(0, 0, initRotation);
        playerMgr.activeBomb++;

        for (int i = 0; i < subCount; ++i)
        {
            pivotRotSpeed[i] = Random.Range(-3f, 3f);
            bulletRotSpeed[i] = -Random.Range(3f, 6f);
            subBullets[i].transform.localPosition = new Vector3(Random.Range(0, 10f), 0, 0);
            subBullets[i].transform.localScale = Vector3.one;
        }
    }

    void Update()
    {
        bool doFollow = activateTime <= followTime;
        bool doRotate = activateTime <= rotateTime;
        bool canStrike = activateTime > strikeBeginTime && activateTime <= strikeBeginTime + strikeDuration;
        bool doExplosion = activateTime > strikeBeginTime + strikeDuration;
        int oldTimeInt = (int)activateTime;
        activateTime += Time.timeScale;
        int newTimeInt = (int)activateTime;

        if (doFollow)
            DoFollow();

        if (doRotate)
            DoRotate();
        else if (canStrike)
            DoStrike();

        if (doExplosion)
        {
            DoExplosion();
        }
        else
        {
            DoSubRotate();
            if (oldTimeInt != newTimeInt)
                DoCollisionDamage();
        }
    }

    private void DoFollow()
    {
        transform.position = playerTrans.position;
        float r = radiusCurve.Evaluate(activateTime / followTime);
        subSetTrans.transform.localPosition = new Vector3(0, r, 0);
    }

    private void DoRotate()
    {
        float rot = rotateCurve.Evaluate(activateTime / rotateTime);
        transform.eulerAngles = new Vector3(0, 0, rot + initRotation);
        if (activateTime > rotateTime)
        {
            strikeBeginPos = subSetTrans.position;
            BattleObjectManager battleManager = BattleStageManager.Instance.GetBattleManager();
            List<BattleObject> enemyList = battleManager.GetObjectList(BattleObjectType.Enemy);
            int enemyCount = enemyList != null ? enemyList.Count : 0;
            if (enemyCount > 0)
            {
                int randIndex = BattleStageManager.Instance.GetRandom(0, enemyCount);
                strikeEndPos = enemyList[randIndex].transform.position;
            }
            else
            {
                Vector3 strikeDir = (strikeBeginPos - playerTrans.position).normalized;
                if (strikeDir.x != 0 && Mathf.Abs(strikeDir.y / strikeDir.x) < 0.75f)
                {
                    float deltaX = strikeDir.x > 0 ? 320f - strikeBeginPos.x : 320f + strikeBeginPos.x;
                    strikeDir *= deltaX / Mathf.Abs(strikeDir.x);
                    strikeEndPos = strikeBeginPos + strikeDir;
                }
                else
                {
                    float deltaY = strikeDir.y > 0 ? 240f - strikeBeginPos.y : 240f + strikeBeginPos.y;
                    strikeDir *= deltaY / Mathf.Abs(strikeDir.y);
                    strikeEndPos = strikeBeginPos + strikeDir;
                }
            }
        }
    }

    private void DoStrike()
    {
        float strikeLerp = strikeCurve.Evaluate((activateTime - strikeBeginTime) / strikeDuration);
        subSetTrans.position = Vector3.Lerp(strikeBeginPos, strikeEndPos, strikeLerp);
        if (activateTime > strikeBeginTime + strikeDuration)
        {
            BattleStageManager.Instance.PlayCameraShake(30f, 0.1f, 0.1f);
            BattleStageManager.Instance.RangeEnemyDamage(subSetTrans.position, explosionRadius, explosionDamage);
            BattleStageManager.Instance.RangeBulletEliminate(subSetTrans.position, explosionRadius);
        }
    }

    private void DoExplosion()
    {
        if (activateTime > strikeBeginTime + strikeDuration + explosionDuration)
        {
            BattleStageManager.Instance.GetPlayerManager().activeBomb--;
            BattleStageManager.Instance.DespawnObject(gameObject);
        }
        else
        {
            for (int i = 0; i < subCount; ++i)
            {
                subBullets[i].transform.localPosition += new Vector3(4f * Time.timeScale, 0, 0);
                float scale = 1 - (activateTime - strikeBeginTime - strikeDuration) / explosionDuration;
                subBullets[i].transform.localScale = new Vector3(scale, scale, 0);
            }
        }
    }

    private void DoSubRotate()
    {
        for (int i = 0; i < subCount; ++i)
        {
            subPivots[i].transform.Rotate(0, 0, pivotRotSpeed[i] * Time.timeScale);
            subBullets[i].transform.Rotate(0, 0, bulletRotSpeed[i] * Time.timeScale);
        }
    }

    private void DoCollisionDamage()
    {
        BattleStageManager.Instance.RangeEnemyDamage(subSetTrans.position, collisionRadius, collisionDamage);
        BattleStageManager.Instance.RangeBulletEliminate(subSetTrans.position, collisionRadius);
    }

    void OnDrawGizmos()
    {
        if (activateTime > strikeBeginTime + strikeDuration)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(subSetTrans.position, explosionRadius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(subSetTrans.position, collisionRadius);
        }
    }
}
