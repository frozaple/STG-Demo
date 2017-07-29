using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BreakEffectType
{
    None = 0,
    Blue = 1,
    Red  = 2,
    All  = 3,
}

public class Enemy : BattleObject
{
    private static Color redBreakEffectColor = new Color(1f, 0.5f, 0.5f, 0.5f);
    private static Color blueBreakEffectColor = new Color(0.5f, 0.5f, 1f, 0.5f);
    private static Color greenBreakEffectColor = new Color(0.5f, 1f, 0.5f, 0.5f);
    private static int shortDelayHash = Animator.StringToHash("ShortBreakDelay");
    private static int longDelayHash = Animator.StringToHash("LongBreakDelay");

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
            ProcessDeath();
            destroy = true;
        }
        CheckBorder();
    }

    private void ProcessDeath()
    {
        for (int i = 0; i < powerDrop; i++)
            DropItem("Item/PowerItem", true);
        for (int i = 0; i < scoreDrop; i++)
            DropItem("Item/ScoreItem", true);
        DropSpecialItem();
        DoBreakEffect();
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

    private void DoBreakEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject breakEff = BattleStageManager.Instance.SpawnObject("Enemy/EnemyBreakEffect");
            Vector3 randOff = Random.insideUnitCircle * dropRange * (i + 1);
            breakEff.transform.position = transform.position + randOff;
            breakEff.transform.right = randOff;
            breakEff.GetComponent<SpriteRenderer>().color = blueBreakEffectColor;
            if (i > 0)
                breakEff.GetComponent<Animator>().Play(i > 1 ? longDelayHash : shortDelayHash);
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
