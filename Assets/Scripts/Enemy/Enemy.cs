using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BattleObject
{
    public int hp;
    public MovingBorder movingBorder;
    public int powerDrop;
    public int scoreDrop;
    public int specialDrop;
    public int dropRange;

    void Awake()
    {
        useScript = true;
    }

    void Update()
    {
        if (hp <= 0)
        {
            for (int i = 0; i < powerDrop; i++)
                DropItem("Item/PowerItem", true);
            for (int i = 0; i < scoreDrop; i++)
                DropItem("Item/ScoreItem", true);
            DropSpecialItem();
            destroy = true;
        }
        CheckBorder();
    }

    private void DropSpecialItem()
    {
        if (specialDrop == 1)
            DropItem("Item/LifeItem", false);
        else if (specialDrop == 2)
            DropItem("Item/SpellItem", false);
    }

    private void DropItem(string name, bool randOffset)
    {
        GameObject item = BattleStageManager.Instance.SpawnObject(name);
        if (randOffset)
        {
            int dropX = BattleStageManager.Instance.GetRandom(-dropRange, dropRange);
            int dropY = BattleStageManager.Instance.GetRandom(-dropRange, dropRange);
            item.transform.position = transform.position + new Vector3(dropX, dropY);
        }
        else
        {
            item.transform.position = transform.position;
        }
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
            DamageObject dmgObj = target as DamageObject;
            if (dmgObj != null)
                hp -= dmgObj.Damage;
        }
    }
}
