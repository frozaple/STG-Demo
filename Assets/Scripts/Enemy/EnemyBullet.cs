using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : BattleObject
{
    public float speed;
    public MovingBorder movingBorder;

    void Awake()
    {
        useScript = true;
    }

    void Update()
    {
        transform.Translate(0, speed * Time.timeScale, 0);
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

    override public void OnCollision(BattleObject target)
    {
        destroy = true;
    }
}
