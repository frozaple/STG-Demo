using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleObjectType
{
    Player = 1,
    PlayerBullet = 2,
    Enemy = 3,
    EnemyBullet = 4,
    Item = 5,
    Max = 6,
}

public enum CollisionType
{
    Circle = 1,
    Box = 2,
    Ray = 3,
}

public class BattleObject : MonoBehaviour
{
    [HideInInspector]
    public BattleObjectType objectType;
    [HideInInspector]
    public CollisionType collisionType;
    [HideInInspector]
    public float radius;
    [HideInInspector]
    public float width, height;
    [HideInInspector]
    public bool valid = true;
    protected bool destroy = false;

    protected void OnEnable()
    {
        valid = true;
        destroy = false;
        BattleStageManager.Instance.AddBattleObject(this);
    }

    protected void OnDisable()
    {
        BattleStageManager.Instance.RemoveBattleObject(this);
    }

    void LateUpdate()
    {
        if (destroy)
            BattleStageManager.Instance.DespawnObject(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (collisionType == CollisionType.Circle)
            Gizmos.DrawWireSphere(transform.position, radius);
        else if (collisionType == CollisionType.Box)
            Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
    }

    public bool CheckCollision(BattleObject target)
    {
        CollisionType targetType = target.collisionType;
        switch (collisionType)
        {
            case CollisionType.Circle:
                if (targetType == CollisionType.Circle)
                    return CircleCheck(target);
                else if (targetType == CollisionType.Box)
                    return CircleBoxCheck(this, target);
                break;
            case CollisionType.Box:
                if (targetType == CollisionType.Circle)
                    return CircleBoxCheck(target, this);
                else if (targetType == CollisionType.Box)
                    return BoxCheck(target);
                break;
        }
        return false;
    }

    private bool CircleCheck(BattleObject target)
    {
        Vector3 selfPos = transform.position;
        Vector3 tarPos = target.transform.position;
        Vector3 disVec = tarPos - selfPos;
        float totalRadius = radius + target.radius;
        return disVec.sqrMagnitude <= totalRadius * totalRadius;
    }

    private bool BoxCheck(BattleObject target)
    {
        Vector3 selfPos = transform.position;
        Rect selfRect = new Rect(selfPos.x - width / 2, selfPos.y - height / 2, width, height);
        Vector3 tarPos = target.transform.position;
        Rect tarRect = new Rect(tarPos.x - target.width / 2, tarPos.y - target.height / 2, target.width, target.height);
        return selfRect.Overlaps(tarRect);
    }

    private bool CircleBoxCheck(BattleObject circle, BattleObject box)
    {
        Vector3 circlePos = circle.transform.position;
        Vector3 boxPos = box.transform.position;
        float halfWidth = box.width / 2;
        float halfHeight = box.height / 2;

        bool hOverlap = circlePos.x + circle.radius >= boxPos.x - halfWidth && circlePos.x - circle.radius <= boxPos.x + halfWidth;
        bool vOverlap = circlePos.y + circle.radius >= boxPos.y - halfHeight && circlePos.y - circle.radius <= boxPos.y + halfHeight;

        if (hOverlap && vOverlap)
        {
            Vector3 cornerOffset = new Vector3();

            if (circlePos.x < boxPos.x - halfWidth)
                cornerOffset.x = -halfWidth;
            if (circlePos.x > boxPos.x + halfWidth)
                cornerOffset.x = halfWidth;
            if (circlePos.y < boxPos.y - halfHeight)
                cornerOffset.y = -halfHeight;
            if (circlePos.y > boxPos.y + halfHeight)
                cornerOffset.y = halfHeight;

            if (cornerOffset.x * cornerOffset.y != 0)
            {
                Vector3 cornerDis = boxPos + cornerOffset - circlePos;
                return cornerDis.sqrMagnitude <= circle.radius * circle.radius;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    virtual public void OnCollision(BattleObject target)
    {
    }
}
