using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    PowerItem = 1,
    ScoreItem = 2,
    LifeItem = 3,
    SpellItem = 4,
}

public class Item : BattleObject
{
    static public float autoFlyRangeSq = 24f * 24f;
    static public float autoFlyRangeSlowSq = 48f * 48f;

    public ItemType itemType;
    public Sprite hintSprite;
    public float hintOffset;
    public float dropGravity;
    public float initDropSpeed;
    public float maxDropSpeed;
    public float rotateDuration;
    public AnimationCurve rotationCurve;

    private float autoFlySpeed;
    private float dropSpeed;
    private SpriteRenderer itemHint;
    private float rotateTime;

    new void OnEnable()
    {
        base.OnEnable();
        dropSpeed = initDropSpeed;
        autoFlySpeed = 0;
        rotateTime = 0;
        transform.eulerAngles = Vector3.zero;
    }

    void Update()
    {
        Vector3 playerPos = BattleStageManager.Instance.GetPlayerPos();
        Vector3 pos = transform.position;
        Vector3 dir = playerPos - pos;
        if (autoFlySpeed > 0)
        {
            float dis = dir.magnitude;
            if (dis > 0)
            {
                float flyDis = Mathf.Min(dis, autoFlySpeed);
                pos += dir / dis * flyDis * Time.timeScale;
                transform.position = pos;
            }
        }
        else
        {
            if (playerPos.y > 96f)
            {
                autoFlySpeed = 10f;
                return;
            }
            float rangeSq = Input.GetAxis("Slow") > 0 ? autoFlyRangeSlowSq : autoFlyRangeSq;
            if (dir.sqrMagnitude < rangeSq)
            {
                autoFlySpeed = 5f;
                return;
            }

            if (dropSpeed < maxDropSpeed)
            {
                dropSpeed += dropGravity * Time.timeScale;
                if (dropSpeed > maxDropSpeed)
                    dropSpeed = maxDropSpeed;
            }

            pos.y -= dropSpeed * Time.timeScale;
            if (pos.y > -256)
                transform.position = pos;
            else
                destroy = true;
        }
        if (pos.y > 240)
        {
            if (itemHint == null)
            {
                GameObject hintObj = BattleStageManager.Instance.SpawnObject("Item/ItemHint");
                itemHint = hintObj.GetComponent<SpriteRenderer>();
                itemHint.sprite = hintSprite;
            }
            itemHint.transform.position = new Vector3(pos.x, hintOffset);
            itemHint.color = new Color(1, 1, 1, (320f - pos.y) / 80f);
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
                playerManager.ChangeFirePower(1);
                playerManager.playerScore += 100;
                break;
            case ItemType.ScoreItem:
                playerManager.playerScore += playerManager.maxPoint;
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
