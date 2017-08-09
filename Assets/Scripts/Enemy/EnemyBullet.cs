using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : BattleObject
{
    public float speed;
    public MovingBorder movingBorder;
    private bool selfRotate;
    private Vector3 moveDir;
    new private SpriteRenderer renderer;

    void Awake()
    {
        useScript = true;
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (selfRotate)
        {
            transform.Rotate(0, 0, -3f * Time.timeScale);
            if (speed > 0)
                transform.position += moveDir * speed * Time.timeScale;
        }
        else if (speed > 0)
        {
            transform.Translate(0, speed * Time.timeScale, 0);
        }
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

    public void SetAppearance(int shape, int color)
    {
        Sprite sprite = BattleStageManager.Instance.GetBulletSprite(shape, color);
        renderer.sprite = sprite;
    }

    public void SetSelfRotate(bool rotate)
    {
        selfRotate = rotate;
        if (rotate)
            moveDir = transform.up;
    }

    public void SetRotation(float rotation)
    {
        transform.eulerAngles = new Vector3(0, 0, rotation);
        if (selfRotate)
            moveDir = transform.up;
    }

    public void Eliminate()
    {
        destroy = true;
    }

    override public void OnCollision(BattleObject target)
    {
        destroy = true;
    }
}
