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
    private float shootAccumulation;

    private float invincibleDuration;
    private bool lastBlink;
    private float dyingTime;
    private bool death;
    private bool reborn;

    public float dyingDuration;
    public float rebornStartHeight;
    public float rebornEndHeight;
    public float rebornMoveSpeed;
    public float rebornInvincibleTime;
    public float bombInvincibleTime;

    new private SpriteRenderer renderer;
    private Animator animator;
    private int speedParamID;
    private int slowEffParamID;
    public GameObject slowEffect;

    void Awake()
    {
        shootAccumulation = 0;
        death = false;
        reborn = false;
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        speedParamID = Animator.StringToHash("Horizontal Speed");
        slowEffParamID = Animator.StringToHash("SlowEff Show");
    }

    void Update()
    {
        if (dyingTime > 0)
        {
            UpdateDying();
        }
        else if (death)
        {
            if (reborn)
                DoReborn();
            else
                DoDeath();
        }
        else
        {
            if (invincibleDuration > 0)
                UpdateInvincible();
            PlayerMove();
            PlayerShoot();
        }
    }

    private void DoDeath()
    {
        float a = renderer.color.a;
        a -= deathAlphaDelta * Time.timeScale;
        if (a > 0)
        {
            transform.localScale += deathScaleDelta * Time.timeScale;
            renderer.color = new Color(1, 1, 1, a);
        }
        else
        {
            transform.position = new Vector3(0, rebornStartHeight);
            transform.localScale = Vector3.one;
            renderer.color = Color.white;
            subWeapon.gameObject.SetActive(true);
            subWeapon.transform.position = transform.position;
            reborn = true;

            PlayerStateManager playerMgr = BattleStageManager.Instance.GetPlayerManager();
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
            death = false;
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
            death = true;
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
                renderer.color = blink ? Color.blue : Color.white;
            }
        }
        else
        {
            renderer.color = Color.white;
            invincibleDuration = 0;
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
            while(shootAccumulation >= shootGap)
            {
                shootAccumulation -= shootGap;
                for (int i = 0;i < mainFirePos.Length; i++)
                {
                    GameObject fireBullet = BattleStageManager.Instance.SpawnObject("Player/Bullet/MainBullet");
                    fireBullet.transform.position = mainFirePos[i].transform.position;
                }
                subWeapon.Shoot();
            }
        }
        if (Input.GetAxis("Bomb") > 0)
        {
            PlayerStateManager playerMgr = BattleStageManager.Instance.GetPlayerManager();
            if (playerMgr.playerSpell > 0 && playerMgr.activeBomb == 0)
            {
                playerMgr.playerSpell--;
                invincibleDuration = bombInvincibleTime;
                for (int i = 0; i < 6; ++i)
                    BattleStageManager.Instance.SpawnObject("Player/Bullet/BombBullet");
            }
        }
    }

    override public void OnCollision(BattleObject target)
    {
        if (!death && invincibleDuration <= 0 && dyingTime <= 0)
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
