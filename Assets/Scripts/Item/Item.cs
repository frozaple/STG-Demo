using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    PowerItem,
    ScoreItem,
    MaxPointItem,
    LifeItem,
    SpellItem,
}

public class Item : BattleObject
{
    static private float dropGravity = 0.05f;
    static private float initDropSpeed = -3f;
    static private float maxDropSpeed = 2f;
    static private float autoFlyRangeSq = 24f * 24f;
    static private float autoFlyRangeSlowSq = 48f * 48f;
    static private float itemLineHeight = 96f;
    static private float itemLineFlySpeed = 15f;
    static private float rangeFlySpeed = 5f;
    static private float destroyHeight = -256f;

    static private float hintHeightMin = 240f;
    static private float hintHeightMax = 320f;
    static private float hintHeight = 80f;
    static private AnimationCurve rotationCurve = new AnimationCurve(new Keyframe[] {
        new Keyframe(0, 0, 0, -1080),
        new Keyframe(1, -1080, 0, 0),
    });

    public ItemType itemType;
    public bool autoFly;
    public Sprite hintSprite;
    public float hintOffset;
    public float rotateDuration;

    private float offsetSpeed;
    private float autoFlySpeed;
    private float dropSpeed;
    private SpriteRenderer itemHint;
    private float rotateTime;

    new void OnEnable()
    {
        base.OnEnable();
        dropSpeed = initDropSpeed;
        autoFlySpeed = autoFly ? itemLineFlySpeed : 0;
        if (autoFly)
            offsetSpeed = BattleStageManager.Instance.GetRandom(-10, 10) / 20f;
        rotateTime = 0;
        transform.eulerAngles = Vector3.zero;
    }

    void Update()
    {
        PlayerStateManager playerManager = BattleStageManager.Instance.GetPlayerManager();
        Vector3 playerPos = BattleStageManager.Instance.GetPlayerPos();
        Vector3 pos = transform.position;
        Vector3 dir = playerPos - pos;
        if (autoFlySpeed > 0 && (!autoFly || dropSpeed >= 0))
        {
            float dis = dir.magnitude;
            if (dis > 0)
            {
                float flyDis = Mathf.Min(dis, autoFlySpeed);
                pos += dir / dis * flyDis * Time.timeScale;
                transform.position = pos;
            }
            if (playerManager.playerDead)
            {
                autoFlySpeed = 0;
                dropSpeed = maxDropSpeed;
            }
        }
        else
        {
            if (autoFlySpeed == 0 && !playerManager.playerDead)
            {
                if (playerPos.y > itemLineHeight)
                {
                    autoFlySpeed = itemLineFlySpeed;
                    return;
                }
                float rangeSq = Input.GetAxis("Slow") > 0 ? autoFlyRangeSlowSq : autoFlyRangeSq;
                if (dir.sqrMagnitude < rangeSq)
                {
                    autoFlySpeed = rangeFlySpeed;
                    return;
                }
            }

            if (dropSpeed < maxDropSpeed)
            {
                dropSpeed += dropGravity * Time.timeScale;
                if (dropSpeed > maxDropSpeed)
                    dropSpeed = maxDropSpeed;
            }

            pos.y -= dropSpeed * Time.timeScale;
            if (autoFly)
                pos.x += offsetSpeed * Time.timeScale;
            if (pos.y > destroyHeight)
                transform.position = pos;
            else
                destroy = true;
        }
        if (hintSprite != null && pos.y > hintHeightMin)
        {
            if (itemHint == null)
            {
                GameObject hintObj = BattleStageManager.Instance.SpawnObject("Item/ItemHint");
                itemHint = hintObj.GetComponent<SpriteRenderer>();
                itemHint.sprite = hintSprite;
            }
            itemHint.transform.position = new Vector3(pos.x, hintOffset);
            itemHint.color = new Color(1, 1, 1, (hintHeightMax - pos.y) / hintHeight);
        }
        else if (itemHint != null)
        {
            BattleStageManager.Instance.DespawnObject(itemHint.gameObject);
            itemHint = null;
        }
        if (rotateTime < rotateDuration)
        {
            rotateTime += Time.timeScale;
            float rot = rotationCurve.Evaluate(rotateTime / rotateDuration);
            transform.eulerAngles = new Vector3(0, 0, rot);
        }
    }

    public override void OnCollision(BattleObject target)
    {
        PlayerStateManager playerManager = BattleStageManager.Instance.GetPlayerManager();
        switch (itemType)
        {
            case ItemType.PowerItem:
                bool hyperBonus = false;
                if (playerManager.ChangeFirePower(1))
                    playerManager.playerScore += 100;
                else
                    hyperBonus = playerManager.AddScore(playerManager.maxPoint / 10, autoFlySpeed == itemLineFlySpeed);
                playerManager.ChangeHyperPower(hyperBonus ? 10 : 5);
                break;
            case ItemType.ScoreItem:
                playerManager.AddScore(playerManager.maxPoint, autoFlySpeed == itemLineFlySpeed);
                break;
            case ItemType.MaxPointItem:
                playerManager.playerScore += 100;
                playerManager.maxPoint += 10;
                break;
            case ItemType.LifeItem:
                playerManager.playerLife++;
                break;
            case ItemType.SpellItem:
                playerManager.playerSpell++;
                break;
        }
        destroy = true;
    }
}
