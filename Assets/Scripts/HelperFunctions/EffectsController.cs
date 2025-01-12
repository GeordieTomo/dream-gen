using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsController : MonoBehaviour
{
    public Volume normalFx;
    public Volume ChaoticFx;

    private float fadeAmt = 0f;
    private float fadeSpeed = 0.5f;

    private float speedUpAmt = 1f;

    private Volume volume;
    private Bloom bloomComponent;
    void Awake()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out bloomComponent);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            fadeAmt += Time.deltaTime * fadeSpeed * (1-fadeAmt);
            fadeAmt = Mathf.Clamp(fadeAmt, 0f, 1f);

            normalFx.weight = 1 - fadeAmt;
            ChaoticFx.weight = fadeAmt;

        }
        else if (fadeAmt > 0f)
        {
            fadeAmt -= Time.deltaTime * fadeSpeed * 4f * (1 - fadeAmt);
            fadeAmt = Mathf.Clamp(fadeAmt, 0f, 1f);

            normalFx.weight = 1 - fadeAmt;
            ChaoticFx.weight = fadeAmt;

        }

        if (Input.GetKey(KeyCode.F))
        {
            speedUpAmt += Time.deltaTime;
            speedUpAmt = Mathf.Clamp(speedUpAmt, 0.125f, 8f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            speedUpAmt -= Time.deltaTime;
            speedUpAmt = Mathf.Clamp(speedUpAmt, 0.125f, 8f);
        }
        else
        {
            if (speedUpAmt < 1)
            {
                speedUpAmt += Time.deltaTime*2f;
                speedUpAmt = Mathf.Clamp(speedUpAmt, 0.125f, 1f);
            }
            else if (speedUpAmt > 1)
            {
                speedUpAmt -= Time.deltaTime * 2f;
                speedUpAmt = Mathf.Clamp(speedUpAmt, 1f, 8f);
            }
        }

        Time.timeScale = speedUpAmt;
    }

    public void SetBloom(float newVal)
    {
        bloomComponent.intensity.value = newVal;
    }
}
