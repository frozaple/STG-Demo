using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {
    public GameObject lifeUI;
    public GameObject spellUI;
    public GameObject scoreUI;
    public GameObject maxPointUI;

    public Image[] imgPowerNum;
    public Sprite[] UINumSprite;

    private Image[] imgLife;
    private Image[] imgSpell;
    private Image[] imgScoreDot;
    private Image[] imgScoreNum;
    private Image[] imgMaxPointNum;

    private int lastLife;
    private int lastSpell;
    private int lastPower;
    private int lastScore;
    private int lastMaxPoint;

    private PlayerStateManager playerManager;

    void Start()
    {
        imgLife = lifeUI.GetComponentsInChildren<Image>();
        imgSpell = spellUI.GetComponentsInChildren<Image>();
        imgScoreDot = scoreUI.transform.FindChild("Dot").GetComponentsInChildren<Image>();
        imgScoreNum = scoreUI.transform.FindChild("Num").GetComponentsInChildren<Image>();
        imgMaxPointNum = maxPointUI.GetComponentsInChildren<Image>();

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
        if (lastScore != playerManager.playerScore)
        {
            lastScore = playerManager.playerScore;
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
