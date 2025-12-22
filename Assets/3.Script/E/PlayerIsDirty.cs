
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsDirty : MonoBehaviour
{
    //Player의 청결도가 30% 이하가 되면 이 스크립트가 진행된다.
    //청결도가 점차 줄어들 적 마다 시야가 줄어든다.
    //청결도가 0 이하가 되면 카메라를 움직이며 쓰러지는 연출을 한다
    //연출이 끝나면 위치를 시작지점으로 전환하고(위치 회전)
    //현재 보유하고 있는 쓰레기를 제거한다.
    //(작은 쓰레기는 사라짐, 큰 쓰레기는 떨어트림)
    [SerializeField] private Camera camera;
    [SerializeField] private float zoominFov = 59f;
    [SerializeField] private float zoomoutFov = 60f;
    [SerializeField] private float zoomDuration = 0.2f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private VignetteController vignetteController;
    [SerializeField] private PlayerIsFaint playerIsFaint;

    [SerializeField] private float maxBeat;
    [SerializeField] private float minBeat;
    private bool isDirtyEffectActive;
    private Coroutine zoomDelayCoroutine;
    private float delay = 0;

    private void Awake()
    {
        if (camera == null)
            camera = Camera.main;
        isDirtyEffectActive = false;
        maxBeat = 2f;
        minBeat = 0.46f;
    }
    private void StartDirtyEffect()
    {
        if (zoomDelayCoroutine != null) return;

        isDirtyEffectActive = true;
        zoomDelayCoroutine = StartCoroutine(zoomDelay_co());
    }

    public void StopDirtyEffect()
    {
        isDirtyEffectActive = false;

        if (zoomDelayCoroutine != null)
        {
            StopCoroutine(zoomDelayCoroutine);
            zoomDelayCoroutine = null;
        }
    }
    private IEnumerator ChangeFovWithCurve(float fromFov, float toFov)
    {
        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)  
        {
            elapsedTime += Time.deltaTime;
            //Debug.Log($"elapsed: {elapsedTime}");
            float normalizedTime = elapsedTime / zoomDuration;
            float curveValue = zoomCurve.Evaluate(normalizedTime);
            float currentFov = Mathf.Lerp(fromFov, toFov, curveValue);
            camera.fieldOfView = currentFov;
            //Debug.Log($"Camera.fieldOfView: {Camera.fieldOfView}");
            yield return null;
        }
        camera.fieldOfView = toFov;
        //ZoomOut();
    }
    private IEnumerator zoomDelay_co()
    {
        while (isDirtyEffectActive)    // 세척 후 청결도가 올라가면 코루틴을 정지시킨다.
        {
            yield return StartCoroutine(ChangeFovWithCurve(camera.fieldOfView, zoominFov));
            yield return new WaitForSeconds(0.05f);
            yield return StartCoroutine(ChangeFovWithCurve(camera.fieldOfView, zoomoutFov));
            float elapsed = 0f;
            while (elapsed < delay)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        zoomDelayCoroutine = null;
    }
    public void CalDelay(float cleanliness)
    {
        if (cleanliness <= 0)
        {
            StopDirtyEffect();
            playerIsFaint.StartPassOutEffect();
            return;
        }
        if (cleanliness > 30)
        {
            Debug.Log("더러움 연출 구간 아님");
            StopDirtyEffect();
            return;
        }
        delay = Mathf.Lerp(maxBeat, minBeat, Mathf.Clamp01(Mathf.Abs(cleanliness - 30f) / 30f));
        StartDirtyEffect();
    }
    //public void JudgeChanging(float cleanliness)
    //{
    //    if (!lastCleanliness.Equals(0))//처음이 아니라면 코루틴이 종료 되기까지 기달리기
    //    {
    //        //우선 바뀌었다고 말해주기 -> 코루틴 종료 신호
    //        changeCleanliness = true;
    //        while (zoomDelayCoroutine != null)
    //        {

    //        }
    //    }
    //    else//처음 청결도가 들어올 때는 바로 코루틴 실행
    //    {
            
    //    }
    //    lastCleanliness = cleanliness;
    //    changeCleanliness = false;
    //    CalDelay(lastCleanliness);
    //}
}
///씻었을때
///lastCleanliness = 0
///changeCleanliness = true