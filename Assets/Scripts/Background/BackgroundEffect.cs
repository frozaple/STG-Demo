using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundEffect : MonoBehaviour
{
    public Shader effectShader;
    public RenderTexture backgroundTexture;
    private Material effectMaterial;

    private float waveTime;
    private float waveDuration;
    private float waveSpeed;

    private float lastColor;
    private PlayerStateManager playerMgr;

    void Awake()
    {
        Renderer renderer = GetComponent<MeshRenderer>();
        effectMaterial = new Material(effectShader);
        renderer.material = effectMaterial;
        effectMaterial.SetTexture("_MainTex", backgroundTexture);

        waveTime = 0;
        waveDuration = 0;
    }

    void Start()
    {
        lastColor = 1;
        playerMgr = BattleStageManager.Instance.GetPlayerManager();
    }

    void Update ()
    {
        UpdateWave();
        UpdateBackColor();
	}

    private void UpdateWave()
    {
        if (waveTime < waveDuration)
        {
            waveTime += Time.timeScale;
            if (waveTime >= waveDuration)
                effectMaterial.DisableKeyword("WAVE_EFFECT");
            else
                effectMaterial.SetFloat("_WaveRadius", waveTime * waveSpeed);
        }
    }

    private void UpdateBackColor()
    {
        float tarColor = 1f;
        if (playerMgr.activeHyper > 0)
            tarColor -= 0.25f;

        bool doSet = lastColor != tarColor;
        if (lastColor < tarColor)
            lastColor = Mathf.Min(lastColor + 0.01f, tarColor);
        else if (lastColor > tarColor)
            lastColor = Mathf.Max(lastColor - 0.01f, tarColor);

        if (doSet)
            effectMaterial.SetFloat("_Color", lastColor);
    }

    void OnDestroy()
    {
        DestroyImmediate(effectMaterial);
    }

    public void AddWaveEffect(Vector2 center, float spd, float duration, float width, float offset)
    {
        waveTime = 0;
        waveSpeed = spd;
        waveDuration = duration;
        effectMaterial.EnableKeyword("WAVE_EFFECT");
        effectMaterial.SetVector("_WaveVector", new Vector4(center.x, center.y, width, offset));
    }
}
