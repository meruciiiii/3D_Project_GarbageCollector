
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteController : MonoBehaviour
{
    //오염 상태에 따라 태두리 생성 제어 스크립트
    [SerializeField] private Volume volume;
    [SerializeField] private float zoomVignetteIntensity = 0.47f;
    [SerializeField] private float vignetteLerpSpeed = 1f;
    private Vignette vignette;
    private float targetSmoothness = 0.0f;
    private Coroutine OnZoom;
    
    private void Start()
    {
        volume.profile.TryGet(out vignette);
        vignette.intensity.value = zoomVignetteIntensity;
        //Debug.Log(volume.name);
        //Debug.Log(vignette.name);   
    }
    public void SetVignetteByCleanliness(float cleanliness)
    {
        if (cleanliness <= 0 || cleanliness > 30)
        {
            Debug.Log("잘못 들어왔습니다.");
            return;
        }
        targetSmoothness = Mathf.Clamp01(Mathf.Abs(cleanliness - 30f) / 30f);

        StartChangeSmoothness(targetSmoothness);
    }
    public void OnWash()
    {
        StartChangeSmoothness(0f);
    }
    private void StartChangeSmoothness(float targetSmoothness)
    {
        if (OnZoom != null)
        {
            StopCoroutine(OnZoom);
        }
        OnZoom = StartCoroutine(OnZoom_co(targetSmoothness));
    }
    private IEnumerator OnZoom_co(float targetSmoothness)
    {
        while (Mathf.Abs( vignette.smoothness.value- targetSmoothness )>= 0.001f)
        {
            vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, targetSmoothness, Time.deltaTime * vignetteLerpSpeed);
            yield return null;
        }
        vignette.smoothness.value = targetSmoothness;
        OnZoom = null;
    }
}
