using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDeltaUI : MonoBehaviour {
    private const float scoreNumGap = 10f;
    private SpriteRenderer[] imgScoreNum;
    private Color scoreColor;
    private int scoreNumLength;
    public float activeTime;

    void Awake()
    {
        imgScoreNum = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        float oldTime = activeTime;
        activeTime += Time.deltaTime;
        float fadeOutTime = activeTime - 0.8f;
        for (int i = 0; i < scoreNumLength; ++i)
        {
            int timeIndex = scoreNumLength - 1 - i;
            float fadeInTime = oldTime - timeIndex * 0.1f;
            if (fadeInTime > 0 && fadeInTime <= 0.1f)
            {
                scoreColor.a = activeTime > (timeIndex + 1) * 0.1f ? 1 : fadeInTime / 0.1f;
                imgScoreNum[i].color = scoreColor;
            }
            if (fadeOutTime > 0)
            {
                scoreColor.a = 1 - fadeOutTime / 0.5f;
                imgScoreNum[i].color = scoreColor;
            }
        }
        if (fadeOutTime > 0.5f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Vector3 pos = transform.position;
            pos.y += 16f * Time.deltaTime;
            transform.position = pos;
        }
    }

    private void Reset()
    {
        for (int i = 0; i < imgScoreNum.Length; ++i)
            imgScoreNum[i].color = Color.clear;
        activeTime = 0;
    }

    public void SetScore(int score, bool full, Sprite[] numSprite)
    {
        Reset();
        scoreColor = full ? Color.yellow : Color.white;
        scoreNumLength = 1;
        for (int i = 0; i < imgScoreNum.Length; ++i)
        {
            imgScoreNum[i].enabled = score > 0 || i == 0;
            if (score > 0)
            {
                if (i > 0)
                    scoreNumLength++;
                imgScoreNum[i].sprite = numSprite[score % 10];
                score /= 10;
            }
        }
        float offset = (scoreNumLength - 1) * scoreNumGap / 2;
        for (int i = 0; i < scoreNumLength; ++i)
            imgScoreNum[i].transform.localPosition = new Vector3(offset - i * scoreNumGap, 0, 0);
    }
}
