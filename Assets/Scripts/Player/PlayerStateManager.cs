using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager
{
    private GameObject playerObj;
    private SubWeapon subWeapon;

    public int playerLife;
    public int playerSpell;
    public int firePower;
    public int playerScore;
    public int maxPoint;

    public void InitPlayer()
    {
        Transform stageTrans = BattleStageManager.Instance.transform;
        playerObj = GameObject.Instantiate(Resources.Load<GameObject>("Player/Player"));
        GameObject subWeaponObj = GameObject.Instantiate(Resources.Load<GameObject>("Player/SubWeapon"));
        playerObj.transform.SetParent(stageTrans);
        subWeaponObj.transform.SetParent(stageTrans);

        subWeapon = subWeaponObj.GetComponent<SubWeapon>();
        playerObj.GetComponent<PlayerController>().subWeapon = subWeapon;
        subWeapon.playerTrans = playerObj.transform;

        playerLife = 2;
        playerSpell = 3;
        firePower = 100;
        subWeapon.SetTamaNum(1);

        playerScore = 0;
        maxPoint = 10000;
    }

    public float GetPlayerAngle(float posX, float posY)
    {
        Vector3 disVec = playerObj.transform.localPosition - new Vector3(posX, posY, 0);
        float angle = Vector3.Angle(Vector3.up, disVec);
        return disVec.x < 0 ? angle : -angle;
    }

    public void ChangeFirePower(int delta)
    {
        firePower = Mathf.Clamp(firePower + delta, 100, 400);
        subWeapon.SetTamaNum(firePower / 100);
    }
}
