using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public float lifeDuration;
    public bool translateMove;
    public Vector3 moveSpeed;
    public bool alphaLerp;

    float activeDuration;
    new SpriteRenderer renderer;
    Color cachedColor;

    private void Awake()
    {
        renderer = gameObject.GetComponent<SpriteRenderer>();
        cachedColor = renderer.color;
    } 

    void OnEnable()
    {
        activeDuration = 0;
        if (alphaLerp)
        {
            cachedColor.a = 1;
            renderer.color = cachedColor;
        }
    }

    void Update () {
        activeDuration += Time.timeScale;
        if (translateMove)
            transform.Translate(moveSpeed * Time.timeScale);
        else
            transform.position += moveSpeed * Time.timeScale;

        if (alphaLerp)
        {
            cachedColor.a = 1 - activeDuration / lifeDuration;
            renderer.color = cachedColor;
        }

        if (activeDuration > lifeDuration)
            BattleStageManager.Instance.DespawnObject(gameObject);
	}
}
