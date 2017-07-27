﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovingBorder
{
    public float left, right, top, bottom;
}

public class PlayerController : BattleObject
{
    public float moveSpeed;
    public float slowMoveSpeed;
    public MovingBorder movingBorder;

    public SubWeapon subWeapon;
    public float subWeaponLerpSpeed;
    
    public Transform[] mainFirePos;
    public float shootGap;
    private float shootAccumulation = 0;

    private Animator animator;
    private int speedParamID;
    private int slowEffParamID;
    public GameObject slowEffect;

    void Awake()
    {
        animator = GetComponent<Animator>();
        speedParamID = Animator.StringToHash("Horizontal Speed");
        slowEffParamID = Animator.StringToHash("SlowEff Show");
    }

    void Update()
    {
        PlayerMove();
        PlayerShoot();
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
    }
}
