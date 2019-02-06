using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShootAngleInfo
{
    public float angle;
}

[System.Serializable]
public struct ShootAngleInfoSet
{
    public ShootAngleInfo[] angleInfo;
}

[System.Serializable]
public struct TamaPosInfo
{
    public Vector2 normalPos, slowPos;
}

[System.Serializable]
public struct TamaPosInfoSet
{
    public TamaPosInfo[] posInfo;
}

public class SubWeapon : MonoBehaviour {
    public Transform playerTrans;
    public float followPlayerLerp;
    public int tamaNum;
    public GameObject[] tama;
    public TamaPosInfoSet[] posInfoSet;
    public ShootAngleInfoSet[] angleInfoSet;
    private float subWeaponLerp;

    public void InternalUpdate()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, playerTrans.localPosition, followPlayerLerp);
	}

    public void SetLerp(float delta)
    {
        if (delta > 0 && subWeaponLerp < 1f || delta < 0 && subWeaponLerp > 0)
        {
            subWeaponLerp = Mathf.Clamp(subWeaponLerp + delta, 0, 1);
            SetTamaPos(subWeaponLerp);
        }
    }

    public void SetTamaPos(float lerp)
    {
        TamaPosInfoSet curPosInfoSet = posInfoSet[tamaNum - 1];
        for (int i = 0; i < tamaNum; i++)
        {
            TamaPosInfo posInfo = curPosInfoSet.posInfo[i];
            tama[i].transform.localPosition = Vector2.Lerp(posInfo.normalPos, posInfo.slowPos, lerp);
        }
    }

    public void SetTamaNum(int num)
    {
        if (tamaNum != num)
        {
            tamaNum = num;
            for (int i = 0; i < tama.Length; i++)
                tama[i].SetActive(i < num);
            SetTamaPos(subWeaponLerp);
        }
    }

    public void Shoot()
    {
        if (subWeaponLerp == 1f)
        {
            for (int i = 0; i < tamaNum; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    GameObject fireNeedle = BattleStageManager.Instance.SpawnObject("Player/Bullet/NeedleBullet");
                    fireNeedle.transform.position = tama[i].transform.position;
                    fireNeedle.transform.Translate(24, 4 - j * 8, 0);
                }
            }
        }
        else if (subWeaponLerp == 0f)
        {
            ShootAngleInfoSet curAngleInfoSet = angleInfoSet[tamaNum - 1];
            for (int i = 0; i < tamaNum; i++)
            {
                GameObject fireBullet = BattleStageManager.Instance.SpawnObject("Player/Bullet/TracingBullet");
                fireBullet.transform.position = tama[i].transform.position;
                fireBullet.transform.eulerAngles = new Vector3(0, 0, curAngleInfoSet.angleInfo[i].angle + 90);
            }
        }
    }
}
