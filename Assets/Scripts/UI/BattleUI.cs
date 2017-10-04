using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour {
    public GameObject lifeUI;
    public GameObject spellUI;
    public GameObject scoreUI;
    public GameObject maxPointUI;
    public GameObject hyperUI;
    public GameObject scoreDeltaUI;

    public SpriteRenderer[] imgPowerNum;
    public Sprite[] UINumSprite;
    public Sprite[] scoreNumSprite;
    public GameObject scoreDeltaObj;

    private SpriteRenderer[] imgLife;
    private SpriteRenderer[] imgSpell;
    private SpriteRenderer[] imgScoreDot;
    private SpriteRenderer[] imgScoreNum;
    private SpriteRenderer[] imgMaxPointNum;
    private SpriteRenderer imgHyper;
    private SpriteRenderer[] imgHyperRune;

    private int lastLife;
    private int lastSpell;
    private int lastPower;
    private int lastMaxPoint;

    private int lastHyper;
    private bool[] doHyperRuneLerp;
    private float hyperRotateSpd;

    private const int scoreDeltaMaxNum = 20;
    private const int scoreIntervalLength = 20;
    private int lastScore;
    private int targetScore;
    private int scoreInterval;
    private ScoreDeltaUI[] comScoreDelta;

    private PlayerStateManager playerManager;

    void Start()
    {
        imgLife = lifeUI.GetComponentsInChildren<SpriteRenderer>();
        imgSpell = spellUI.GetComponentsInChildren<SpriteRenderer>();
        imgScoreDot = scoreUI.transform.FindChild("Dot").GetComponentsInChildren<SpriteRenderer>();
        imgScoreNum = scoreUI.transform.FindChild("Num").GetComponentsInChildren<SpriteRenderer>();
        imgMaxPointNum = maxPointUI.GetComponentsInChildren<SpriteRenderer>();
        imgHyper = hyperUI.transform.FindChild("ImgHyper").GetComponent<SpriteRenderer>();
        imgHyperRune = hyperUI.transform.FindChild("Runes").GetComponentsInChildren<SpriteRenderer>();

        lastScore = -1;
        lastHyper = -1;
        doHyperRuneLerp = new bool[imgHyperRune.Length];
        comScoreDelta = new ScoreDeltaUI[scoreDeltaMaxNum];

        playerManager = BattleStageManager.Instance.GetPlayerManager();
    }

    void Update()
    {
        UpdateLife();
        UpdateSpell();
        UpdatePower();
        UpdateHyper();
        UpdateScore();
        UpdateScoreDelta();
        UpdateMaxPoint();
    }

    private void UpdateLife()
    {
        if (lastLife != playerManager.playerLife)
        {
            lastLife = playerManager.playerLife;
            for (int i = imgLife.Length - 1; i >= 0; i--)
                imgLife[i].enabled = i < lastLife;
        }
    }

    private void UpdateSpell()
    {
        if (lastSpell != playerManager.playerSpell)
        {
            lastSpell = playerManager.playerSpell;
            for (int i = imgSpell.Length - 1; i >= 0; i--)
                imgSpell[i].enabled = i < lastSpell;
        }
    }

    private void UpdatePower()
    {
        if (lastPower != playerManager.firePower)
        {
            lastPower = playerManager.firePower;
            int power = lastPower;
            for (int i = imgPowerNum.Length - 1; i >= 0; i--)
            {
                imgPowerNum[i].sprite = UINumSprite[power % 10];
                power /= 10;
            }
        }
    }

    private void UpdateHyper()
    {
        int hyperLevel = playerManager.hyperPower / 125;
        if (lastHyper != hyperLevel)
        {
            lastHyper = hyperLevel;
            for (int i = doHyperRuneLerp.Length - 1; i >= 0; i--)
                doHyperRuneLerp[i] = true;
        }

        if (playerManager.activeHyper > 0)
            hyperRotateSpd = 16f;
        else if (hyperRotateSpd < lastHyper)
            hyperRotateSpd = lastHyper;
        else if (hyperRotateSpd > lastHyper)
            hyperRotateSpd -= 0.2f;
        if (hyperRotateSpd > 0)
            imgHyper.transform.Rotate(0, 0, -hyperRotateSpd);

        for (int i = imgHyperRune.Length - 1; i >= 0; i--)
        {
            if (doHyperRuneLerp[i])
            {
                float alpha = imgHyperRune[i].color.a;
                if (i < lastHyper)
                {
                    if (alpha < 1)
                        alpha += 0.05f;
                    else
                        doHyperRuneLerp[i] = false;
                }
                else
                {
                    if (alpha > 0)
                        alpha -= 0.05f;
                    else
                        doHyperRuneLerp[i] = false;
                }
                float scale = (2 - Mathf.Clamp(alpha, 0, 1)) * 0.4f;
                imgHyperRune[i].transform.localScale = new Vector3(scale, scale, 1);
                imgHyperRune[i].color = new Color(1, 1, 1, alpha);
            }
        }
    }

    private void UpdateScore()
    {
        if (targetScore != playerManager.playerScore)
        {
            targetScore = playerManager.playerScore;
            scoreInterval = scoreIntervalLength;
        }
        if (lastScore != targetScore)
        {
            if (scoreInterval > 1)
            {
                scoreInterval--;
                if (scoreInterval > 1)
                {
                    int delta = targetScore - lastScore;
                    int divideVal = (1 + scoreInterval) * scoreInterval * 5;
                    lastScore += delta * scoreInterval / divideVal * 10 + 10;
                }
            }
            if (scoreInterval <= 1)
                lastScore = targetScore;

            int score = lastScore;
            int digit = 0;
            for (int i = 0; i < imgScoreNum.Length; i++)
            {
                imgScoreNum[i].enabled = score > 0 || i == 0;
                if (score > 0)
                {
                    imgScoreNum[i].sprite = UINumSprite[score % 10];
                    score /= 10;
                    digit++;
                }
            }
            for (int i = imgScoreDot.Length - 1; i >= 0; i--)
                imgScoreDot[i].enabled = i < (digit - 1) / 3;
        }
    }

    private void UpdateScoreDelta()
    {
        int remainCount = playerManager.scoreDeltaList.Count;
        foreach (ScoreDeltaInfo info in playerManager.scoreDeltaList)
        {
            remainCount--;
            if (remainCount < scoreDeltaMaxNum)
            {
                int index = 0;
                float maxActiveTime = 0;
                for (int i = 0; i < scoreDeltaMaxNum; ++i)
                {
                    ScoreDeltaUI com = comScoreDelta[i];
                    if (com == null)
                    {
                        GameObject deltaObj = Instantiate(scoreDeltaObj);
                        deltaObj.transform.parent = scoreDeltaUI.transform;
                        comScoreDelta[i] = deltaObj.GetComponent<ScoreDeltaUI>();
                        index = i;
                        break;
                    }
                    else if (!com.gameObject.activeSelf)
                    {
                        index = i;
                        break;
                    }
                    else if (com.activeTime > maxActiveTime)
                    {
                        maxActiveTime = com.activeTime;
                        index = i;
                    }
                }
                ScoreDeltaUI tarCom = comScoreDelta[index];
                tarCom.gameObject.SetActive(true);
                tarCom.SetScore(info.delta, info.full, scoreNumSprite);
                tarCom.transform.position = playerManager.GetPlayerTrans().position;
            }
        }
        playerManager.scoreDeltaList.Clear();
    }

    private void UpdateMaxPoint()
    {
        if (lastMaxPoint != playerManager.maxPoint)
        {
            lastMaxPoint = playerManager.maxPoint;
            int point = lastMaxPoint;
            for (int i = 0; i < imgMaxPointNum.Length; i++)
            {
                imgMaxPointNum[i].enabled = point > 0 || i == 0;
                if (point > 0)
                {
                    imgMaxPointNum[i].sprite = UINumSprite[point % 10];
                    point /= 10;
                }
            }
        }
    }
}
