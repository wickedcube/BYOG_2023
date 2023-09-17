using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPAnim : MonoBehaviour
{
    [SerializeField] private Volume volume;

    private void Start()
    {
        StartCoroutine(Vignette());
        StartCoroutine(SaturationAndContrast());
    }

    IEnumerator Vignette()
    {
        yield return new WaitForSeconds(6);
        Vignette vig;
        volume.profile.TryGet(typeof(Vignette), out vig);
        float timeStep = 0;
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime;
            vig.intensity.value = Mathf.Lerp(0, 0.35f, timeStep);
            yield return new WaitForEndOfFrame();
        }
    }
    
    IEnumerator SaturationAndContrast()
    {
        yield return new WaitForSeconds(7);
        ColorAdjustments adjustments;
        volume.profile.TryGet(typeof(ColorAdjustments), out adjustments);
        float timeStep = 0;
        while (timeStep <= 1)
        {
            timeStep += Time.deltaTime / 2;
            adjustments.saturation.value = Mathf.Lerp(0, -100f, timeStep);
            adjustments.contrast.value = Mathf.Lerp(0, 100f, timeStep);
            yield return new WaitForEndOfFrame();
        }
    }
}
