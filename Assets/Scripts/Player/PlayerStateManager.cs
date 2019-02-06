using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ScoreDeltaInfo
{
    public int delta;
    public bool full;

    public ScoreDeltaInfo(int delta, bool full)
    {
        this.delta = delta;
        this.full = full;
    }
}

public class PlayerStateManager
{
    static private float fullScoreHeight = 80f;
    static private float scoreHeightScale = 320f;

    private GameObject playerObj;
    private SubWeapon subWeapon;

    public int playerLife;
    public int playerSpell;
    public int firePower;
    public int hyperPower;
    public int playerScore;
    public int maxPoint;

    public List<ScoreDeltaInfo> scoreDeltaList;

    public bool playerDead;
    public float activeHyper;
    public List<BombBullet> activeBomb;

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
        subWeapon.SetTamaNum(1);
        subWeapon.SetTamaPos(0);

        playerLife = 2;
        playerSpell = 3;
        firePower = 100;
        hyperPower = 0;

        playerScore = 0;
        maxPoint = 10000;

        scoreDeltaList = new List<ScoreDeltaInfo>();
        activeBomb = new List<BombBullet>();
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

    public bool ChangeFirePower(int delta)
    {
        int oldPower = firePower;
        firePower = Mathf.Clamp(firePower + delta, 100, 400);
        subWeapon.SetTamaNum(firePower / 100);
        return oldPower < 400;
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

    public void InternalUpdate()
    {
        if (activeHyper > 0)
        {
            activeHyper -= Time.timeScale;
            if (activeHyper <= 0)
                hyperPower = 0;
        }
        if (activeBomb.Count > 0)
        {
            for (int i = activeBomb.Count - 1; i >= 0; --i)
                activeBomb[i].InternalUpdate();
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

    public bool AddScore(int score, bool full)
    {
        float rate = 1f;
        if (!full)
        {
            float heightDelta = fullScoreHeight - playerObj.transform.position.y;
            full = heightDelta <= 0;
            rate = Mathf.Clamp(1 - heightDelta / scoreHeightScale, 0, 1);
        }
        int scoreDelta = (int)(score * rate / 10) * 10;
        scoreDeltaList.Add(new ScoreDeltaInfo(scoreDelta, full));
        playerScore += scoreDelta;
        return full;
    }
}
