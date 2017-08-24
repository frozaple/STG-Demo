﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    private Vector3 origin;
    private float shakeDuration;
    private float shakeX;
    private float shakeY;

    public void DoShake(float duration, float x, float y)
    {
        shakeDuration = duration;
        shakeX = x;
        shakeY = y;
    }

    void Start()
    {
        origin = transform.position;
    }

    void Update() {
        if (shakeDuration > 0)
        {
            int oldVal = (int)shakeDuration;
            shakeDuration -= Time.timeScale;
            int newVal = (int)shakeDuration;
            if (newVal != oldVal)
            {
                if (newVal != 0)
                    transform.position = origin + new Vector3(shakeX * Random.Range(-1f, 1f), shakeY * Random.Range(-1f, 1f));
                else
                    transform.position = origin;
            }
        }
	}
}
