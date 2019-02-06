using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovingBorder
{
    public float left, right, top, bottom;
}

public class PlayerController : BattleObject
{
    static private float deathAlphaDelta = 0.05f;
    static private Vector3 deathScaleDelta = new Vector3(-0.05f, 0.05f);

    public float moveSpeed;
    public float slowMoveSpeed;
    public MovingBorder movingBorder;

    public SubWeapon subWeapon;
    public float subWeaponLerpSpeed;
    
    public Transform[] mainFirePos;
    public float shootGap;
    public float hyperShootGap;
    private float shootAccumulation;

    private float invincibleDuration;
    private bool lastBlink;
    private float dyingTime;
    private bool reborn;

    public float dyingDuration;
    public float rebornStartHeight;
    public float rebornEndHeight;
    public float rebornMoveSpeed;
    public float rebornInvincibleTime;
    public float bombInvincibleTime;

    public float deathEliminateDuration;
    public float deathEliminateRadiusSpeed;
    public float deathEliminateDelay;
    public int deathEliminateDamage;

    private PlayerStateManager playerMgr;
    private float hyperActiveTime;
    private float hyperEffectScale;

    private SpriteRenderer playerRenderer;
    private Animator animator;
    private int speedParamID;
    private int slowEffParamID;
    public GameObject slowEffect;
    public GameObject hyperEffect;

    void Awake()
    {
        shootAccumulation = 0;
        reborn = false;
        playerMgr = BattleStageManager.Instance.GetPlayerManager();
        playerRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        speedParamID = Animator.StringToHash("Horizontal Speed");
        slowEffParamID = Animator.StringToHash("SlowEff Show");
    }

    public override void InternalUpdate()
    {
        if (playerMgr.playerDead)
        {
            if (reborn)
                DoReborn();
            else
                DoDeath();
        }
        else
        {
            PlayerBomb();
            if (dyingTime > 0)
            {
                UpdateDying();
            }
            else
            {
                if (invincibleDuration > 0)
                    UpdateInvincible();
                PlayerMove();
                PlayerShoot();
            }
        }

        subWeapon.InternalUpdate();
        if (hyperEffect.activeSelf)
            UpdateHyperEffect();
    }

    private void DoDeath()
    {
        float a = playerRenderer.color.a;
        a -= deathAlphaDelta * Time.timeScale;
        if (a > 0)
        {
            transform.localScale += deathScaleDelta * Time.timeScale;
            playerRenderer.color = new Color(1, 1, 1, a);
        }
        else
        {
            RangeTask newTask = new RangeTask(transform.position, deathEliminateDelay, deathEliminateDuration, deathEliminateRadiusSpeed, false, deathEliminateDamage);
            BattleStageManager.Instance.AddRangeTask(newTask);
            transform.position = new Vector3(0, rebornStartHeight);
            transform.localScale = Vector3.one;
            playerRenderer.color = Color.white;
            subWeapon.gameObject.SetActive(true);
            subWeapon.transform.position = transform.position;
            reborn = true;
            
            playerMgr.playerLife = Mathf.Max(playerMgr.playerLife - 1, 0);
            playerMgr.playerSpell = 3;
        }
    }

    private void DoReborn()
    {
        Vector3 pos = transform.position;
        pos.y += rebornMoveSpeed * Time.timeScale;
        if (pos.y >= rebornEndHeight)
        {
            pos.y = rebornEndHeight;
            invincibleDuration = rebornInvincibleTime;
            playerMgr.SetPlayerDead(false);
            reborn = false;
            valid = true;
        }
        transform.position = pos;
    }

    private void UpdateDying()
    {
        dyingTime -= Time.timeScale;
        if (dyingTime <= 0)
        {
            dyingTime = 0;
            BattleStageManager.Instance.PlayDeathEffect(transform.position);
            subWeapon.gameObject.SetActive(false);
            if (slowEffect.activeSelf)
                slowEffect.SetActive(false);
            valid = false;
            playerMgr.SetPlayerDead(true);
        }
    }

    private void UpdateInvincible()
    {
        invincibleDuration -= Time.timeScale;
        if (invincibleDuration > 0)
        {
            bool blink = ((int)invincibleDuration) / 3 % 2 > 0;
            if (lastBlink != blink)
            {
                lastBlink = blink;
                playerRenderer.color = blink ? Color.blue : Color.white;
            }
        }
        else
        {
            playerRenderer.color = Color.white;
            invincibleDuration = 0;
        }
    }

    private void UpdateHyperEffect()
    {
        if (playerMgr.activeHyper > 0)
        {
            float scale = playerMgr.activeHyper / hyperActiveTime;
            float rotateSpd = -6f;
            if (hyperEffectScale < 1)
            {
                rotateSpd = -12f;
                hyperEffectScale = Mathf.Min(hyperEffectScale + 0.02f * Time.timeScale, 1);
                scale *= hyperEffectScale;
            }
            hyperEffect.transform.localScale = new Vector3(scale, scale, 1);
            hyperEffect.transform.Rotate(0, 0, rotateSpd * Time.timeScale);
        }
        else
        {
            hyperEffect.SetActive(false);
        }
    }

    private void PlayerMove()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float slowInput = Input.GetAxis("Slow");

        animator.SetFloat(speedParamID, horizontalInput);
        Vector3 moveVector = new Vector3(horizontalInput, verticalInput, 0);
        moveVector.Normalize();
        if (slowInput > 0)
        {
            subWeapon.SetLerp(subWeaponLerpSpeed * Time.timeScale);
            transform.Translate(moveVector * slowMoveSpeed * Time.timeScale);
            if (!slowEffect.activeSelf)
            {
                animator.SetTrigger(slowEffParamID);
                slowEffect.SetActive(true);
            }
        }
        else
        {
            subWeapon.SetLerp(-subWeaponLerpSpeed * Time.timeScale);
            transform.Translate(moveVector * moveSpeed * Time.timeScale);
            if (slowEffect.activeSelf)
                slowEffect.SetActive(false);
        }

        moveVector.x = Mathf.Clamp(transform.localPosition.x, movingBorder.left, movingBorder.right);
        moveVector.y = Mathf.Clamp(transform.localPosition.y, movingBorder.bottom, movingBorder.top);
        transform.localPosition = moveVector;
    }

    private void PlayerShoot()
    {
        if (Input.GetAxis("Fire") > 0)
        {
            shootAccumulation += Time.timeScale;
            float gap = playerMgr.activeHyper > 0 ? hyperShootGap : shootGap;
            while(shootAccumulation >= gap)
            {
                shootAccumulation -= gap;
                for (int i = 0;i < mainFirePos.Length; i++)
                {
                    GameObject fireBullet = BattleStageManager.Instance.SpawnObject("Player/Bullet/MainBullet");
                    fireBullet.transform.position = mainFirePos[i].transform.position;
                }
                subWeapon.Shoot();
            }
        }
        if (Input.GetAxis("Hyper") > 0)
        {
            if (playerMgr.hyperPower >= 625 && playerMgr.activeHyper <= 0)
            {
                hyperActiveTime = playerMgr.hyperPower * 12 / 10;
                playerMgr.activeHyper = hyperActiveTime;
                BattleStageManager.Instance.AddRangeTask(new RangeTask(transform.position, 0, 45f, 20f, true, 0));
                BattleStageManager.Instance.AddWaveEffect(transform.position, 20f, 45f, 30f, 8f);
                hyperEffect.SetActive(true);
                hyperEffectScale = 0;
                hyperEffect.transform.localScale = Vector3.zero;
            }
        }
    }

    private void PlayerBomb()
    {
        if (Input.GetAxis("Bomb") > 0)
        {
            if (playerMgr.playerSpell > 0 && playerMgr.activeBomb.Count == 0)
            {
                playerMgr.playerSpell--;
                dyingTime = 0;
                invincibleDuration = bombInvincibleTime;
                for (int i = 0; i < 6; ++i)
                    BattleStageManager.Instance.SpawnObject("Player/Bullet/BombBullet");
            }
        }
    }

    override public void OnCollision(BattleObject target)
    {
        if (!playerMgr.playerDead && invincibleDuration <= 0 && dyingTime <= 0)
        {
            if (target.objectType == BattleObjectType.Enemy || target.objectType == BattleObjectType.EnemyBullet)
            {
                GameObject deadCircle = BattleStageManager.Instance.SpawnObject("Player/PlayerDeadCircle");
                deadCircle.transform.position = transform.position;
                dyingTime = dyingDuration;
            }
        }
    }
}
