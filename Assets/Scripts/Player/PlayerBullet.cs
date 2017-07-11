using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : BattleObject
{
    public float flySpeed;
    public bool mainBullet;
    public int damage;

    void Update ()
    {
        if (transform.position.y < 240)
            transform.Translate(flySpeed * Time.timeScale, 0, 0);
        else
            destroy = true;
    }

    public override void OnCollision(BattleObject target)
    {
        GameObject bulletEff;
        if (mainBullet)
        {
            bulletEff = BattleStageManager.Instance.SpawnObject("Player/Bullet/MainBulletEffect");
        }
        else
        {
            bulletEff = BattleStageManager.Instance.SpawnObject("Player/Bullet/NeedleBulletEffect");
            bulletEff.transform.eulerAngles = new Vector3(0, 0, 90f + Random.Range(-10f, 10f));
        }
        if (bulletEff != null)
        {
            Vector3 effPos = transform.position;
            effPos.y = target.transform.position.y;
            bulletEff.transform.position = effPos;
        }
        destroy = true;
    }
}
