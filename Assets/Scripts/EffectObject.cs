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
    public bool alphaLerp;

    private bool doMove;
    private bool doScale;
    private float activeDuration;
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

    void OnEnable()
    {
        activeDuration = 0;
        doMove = moveSpeed != Vector3.zero;
        doScale = beginScale != endScale;
        if (doScale)
            transform.localScale = beginScale;
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
        activeDuration += Time.timeScale;

        if (doMove)
        {
            if (translateMove)
                transform.Translate(moveSpeed * Time.timeScale);
            else
                transform.position += moveSpeed * Time.timeScale;
        }

        if (doScale)
        {
            transform.localScale = Vector3.Lerp(beginScale, endScale, activeDuration / lifeDuration);
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
