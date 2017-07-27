using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour {
    public GameObject lifeUI;
    public GameObject spellUI;
    public GameObject scoreUI;
    public GameObject maxPointUI;

    public SpriteRenderer[] imgPowerNum;
    public Sprite[] UINumSprite;

    private SpriteRenderer[] imgLife;
    private SpriteRenderer[] imgSpell;
    private SpriteRenderer[] imgScoreDot;
    private SpriteRenderer[] imgScoreNum;
    private SpriteRenderer[] imgMaxPointNum;

    private int lastLife;
    private int lastSpell;
    private int lastPower;
    private int lastMaxPoint;

    private const int scoreIntervalLength = 20;
    private int lastScore;
    private int targetScore;
    private int scoreInterval;

    private PlayerStateManager playerManager;

    void Start()
    {
        imgLife = lifeUI.GetComponentsInChildren<SpriteRenderer>();
        imgSpell = spellUI.GetComponentsInChildren<SpriteRenderer>();
        imgScoreDot = scoreUI.transform.FindChild("Dot").GetComponentsInChildren<SpriteRenderer>();
        imgScoreNum = scoreUI.transform.FindChild("Num").GetComponentsInChildren<SpriteRenderer>();
        imgMaxPointNum = maxPointUI.GetComponentsInChildren<SpriteRenderer>();

        lastScore = -1;

        playerManager = BattleStageManager.Instance.GetPlayerManager();
    }

    void Update()
    {
        UpdateLife();
        UpdateSpell();
        UpdatePower();
        UpdateScore();
        UpdateMaxPoint();
    }

    private void UpdateLife()
    {
        if (lastLife != playerManager.playerLife)
        {
            lastLife = playerManager.playerLife;
            for (int i = imgLife.Length - 1; i >= 0; i--)
                imgLife[i].gameObject.SetActive(i < lastLife);
        }
    }

    private void UpdateSpell()
    {
        if (lastSpell != playerManager.playerSpell)
        {
            lastSpell = playerManager.playerSpell;
            for (int i = imgSpell.Length - 1; i >= 0; i--)
                imgSpell[i].gameObject.SetActive(i < lastSpell);
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
                imgScoreNum[i].gameObject.SetActive(score > 0 || i == 0);
                if (score > 0)
                {
                    imgScoreNum[i].sprite = UINumSprite[score % 10];
                    score /= 10;
                    digit++;
                }
            }
            for (int i = imgScoreDot.Length - 1; i >= 0; i--)
                imgScoreDot[i].gameObject.SetActive(i < (digit - 1) / 3);
        }
    }

    private void UpdateMaxPoint()
    {
        if (lastMaxPoint != playerManager.maxPoint)
        {
            lastMaxPoint = playerManager.maxPoint;
            int point = lastMaxPoint;
            for (int i = 0; i < imgMaxPointNum.Length; i++)
            {
                imgMaxPointNum[i].gameObject.SetActive(point > 0 || i == 0);
                if (point > 0)
                {
                    imgMaxPointNum[i].sprite = UINumSprite[point % 10];
                    point /= 10;
                }
            }
        }
    }
}
