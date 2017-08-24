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
    public int hyperPower;
    public int playerScore;
    public int maxPoint;

    public bool playerDead;
    public float activeHyper;
    public int activeBomb;

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
        hyperPower = 0;

        playerScore = 0;
        maxPoint = 10000;
    }

    public Vector3 GetPlayerPos()
    {
        return playerObj.transform.position;
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

    public void ChangeHyperPower(int delta)
    {
        if (activeHyper <= 0)
            hyperPower = Mathf.Clamp(hyperPower + delta, 0, 1000);
    }

    public Transform GetPlayerTrans()
    {
        return playerObj.transform;
    }

    public void Update()
    {
        if (activeHyper > 0)
        {
            activeHyper -= Time.timeScale;
            if (activeHyper <= 0)
                hyperPower = 0;
        }
    }

    public void SetPlayerDead(bool dead)
    {
        playerDead = dead;
        if (dead && activeHyper > 0)
        {
            activeHyper = 0;
            hyperPower = 0;
        }
    }
}
