using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BattleObject
{
    public int hp = 0;
    public MovingBorder movingBorder;

    void Awake()
    {
        useScript = true;
    }

    void Update()
    {
        if (hp <= 0)
            destroy = true;
        CheckBorder();
    }

    private void CheckBorder()
    {
        float posX = transform.localPosition.x;
        float posY = transform.localPosition.y;
        if (posX < movingBorder.left || posX > movingBorder.right ||
            posY < movingBorder.bottom || posY > movingBorder.top)
            destroy = true;
    }

    public override void OnCollision(BattleObject target)
    {
        if (target.objectType == BattleObjectType.PlayerBullet)
        {
            PlayerBullet bulletObj = target as PlayerBullet;
            if (bulletObj != null)
            {
                hp -= bulletObj.damage;
            }
            else
            {
                TracingBullet tracingObj = target as TracingBullet;
                if (tracingObj != null)
                    hp -= tracingObj.damage;
            }
        }
    }
}
