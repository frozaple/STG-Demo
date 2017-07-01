﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracingBullet : BattleObject
{
    public float minSpeed;
    public float maxSpeed;
    public float speedDelta;
    public float angleSpeed;
    public MovingBorder movingBorder;

    private float curSpeed;
    private float radSpeed;
    private float accelCos;

    void Start()
    {
        radSpeed = angleSpeed * Mathf.Deg2Rad;
        accelCos = Mathf.Cos(radSpeed);
    }

    new void OnEnable()
    {
        base.OnEnable();
        curSpeed = minSpeed;
    }

    void Update()
    {
        transform.Translate(curSpeed * Time.timeScale, 0, 0);
        ProcessTracing();
        float posX = transform.localPosition.x;
        float posY = transform.localPosition.y;
        if (posX < movingBorder.left || posX > movingBorder.right ||
            posY < movingBorder.bottom || posY > movingBorder.top)
            destroy = true;
    }

    private void ProcessTracing()
    {
        BattleObjectManager battleManager = BattleStageManager.Instance.GetBattleManager();
        List<BattleObject> enemyList = battleManager.GetObjectList(BattleObjectType.Enemy);

        BattleObject tracingEnemy = null;
        Vector3 disVec = new Vector3();
        if (enemyList != null)
        {
            float lastDisSq = 0;
            foreach (BattleObject enemy in enemyList)
            {
                disVec = enemy.transform.position - transform.position;
                if (tracingEnemy == null || lastDisSq > disVec.sqrMagnitude)
                {
                    tracingEnemy = enemy;
                    lastDisSq = disVec.sqrMagnitude;
                }
            }
        }


        if (tracingEnemy != null)
        {
            disVec.Normalize();
            transform.right = Vector3.RotateTowards(transform.right, disVec, radSpeed, 0f);
            if (Vector3.Dot(transform.right, disVec) < accelCos)
            {
                if (curSpeed > minSpeed)
                {
                    curSpeed -= speedDelta * Time.timeScale;
                    if (curSpeed < minSpeed) curSpeed = minSpeed;
                }
                return;
            }
        }

        if (curSpeed < maxSpeed)
        {
            curSpeed += speedDelta * Time.timeScale;
            if (curSpeed > maxSpeed) curSpeed = maxSpeed;
        }
    }

    public override void OnCollision(BattleObject target)
    {
        GameObject bulletEff = BattleStageManager.Instance.SpawnObject("Player/Bullet/TracingBulletEffect");
        bulletEff.transform.position = transform.position;
        bulletEff.transform.eulerAngles = transform.eulerAngles;
        destroy = true;
    }
}
