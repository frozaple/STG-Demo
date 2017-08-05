using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public float lifeDuration;
    public bool translateMove;
    public Vector3 moveSpeed;
    public Vector3 beginScale;
    public Vector3 endScale;
    public Sprite[] animateSprites;
    public bool alphaLerp;

    private bool doMove;
    private bool doScale;
    private bool doAnimate;

    private float activeDuration;
    private AnimationCurve scaleCurve;
    private int animateIndex;
    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;
    private Color cachedColor;
    private float initAlpha;

    private void Awake()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer is SpriteRenderer)
        {
            spriteRenderer = renderer as SpriteRenderer;
            cachedColor = spriteRenderer.color;
        }
        if (renderer is LineRenderer)
        {
            lineRenderer = renderer as LineRenderer;
            cachedColor = lineRenderer.startColor;
        }
        initAlpha = cachedColor.a;
    }

    public void SetDelay(float delayDuration)
    {
        if (delayDuration > 0)
        {
            activeDuration = -delayDuration;
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            if (lineRenderer != null)
                lineRenderer.enabled = false;
        }
    }

    public void SetScaleCurve(AnimationCurve curve)
    {
        scaleCurve = curve;
    }

    void OnEnable()
    {
        activeDuration = 0;
        doMove = moveSpeed != Vector3.zero;
        doScale = beginScale != endScale;
        doAnimate = animateSprites.Length > 0;
        if (doScale)
            transform.localScale = beginScale;
        if (doAnimate && spriteRenderer != null)
        {
            animateIndex = 0;
            spriteRenderer.sprite = animateSprites[0];
        }
        if (alphaLerp)
        {
            cachedColor.a = initAlpha;
            if (spriteRenderer != null)
                spriteRenderer.color = cachedColor;
            if (lineRenderer != null)
            {
                lineRenderer.startColor = cachedColor;
                lineRenderer.endColor = cachedColor;
            }
        }
    }

    void Update () {
        bool delayed = activeDuration < 0;
        activeDuration += Time.timeScale;
        if (delayed)
        {
            if (activeDuration < 0)
                return;
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            if (lineRenderer != null)
                lineRenderer.enabled = true;
        }

        if (doMove)
        {
            if (translateMove)
                transform.Translate(moveSpeed * Time.timeScale);
            else
                transform.position += moveSpeed * Time.timeScale;
        }

        if (doScale)
        {
            if (scaleCurve != null)
            {
                float scale = scaleCurve.Evaluate(activeDuration / lifeDuration);
                transform.localScale = new Vector3(scale, scale, 1);
            }
            else
            {
                transform.localScale = Vector3.Lerp(beginScale, endScale, activeDuration / lifeDuration);
            }
        }

        if (doAnimate && spriteRenderer != null)
        {
            int curIndex = (int)(activeDuration / lifeDuration * animateSprites.Length);
            curIndex = Mathf.Min(curIndex, animateSprites.Length - 1);
            if (animateIndex != curIndex)
            {
                animateIndex = curIndex;
                spriteRenderer.sprite = animateSprites[animateIndex];
            }
        }

        if (alphaLerp)
        {
            cachedColor.a = (1 - activeDuration / lifeDuration) * initAlpha;
            if (spriteRenderer != null)
                spriteRenderer.color = cachedColor;
            if (lineRenderer != null)
            {
                lineRenderer.startColor = cachedColor;
                lineRenderer.endColor = cachedColor;
            }
        }

        if (activeDuration > lifeDuration)
            BattleStageManager.Instance.DespawnObject(gameObject);
	}
}
