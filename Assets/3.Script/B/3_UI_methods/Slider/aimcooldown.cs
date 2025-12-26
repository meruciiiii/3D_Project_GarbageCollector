using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aimcooldown : MonoBehaviour
{
    [SerializeField] private Slider cooldownSlider;
    private Coroutine chargeCoroutine;

    private void Start()
    {
        // 게임 시작 시 매니저의 신호를 기다리겠다고 등록(구독)함
        if (GameManager.instance != null)
        {
            GameManager.instance.OnWeightIncreased += StartCooldown;
        }
    }

    private void StartCooldown()
    {
        // 무게가 늘어났다는 신호가 올 때만 실행됨
        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        chargeCoroutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        float duration = GameManager.instance.grab_speed;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            cooldownSlider.value = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        cooldownSlider.value = 1f;
        yield return new WaitForEndOfFrame();
        cooldownSlider.value = 0;
        chargeCoroutine = null;
    }

    private void OnDestroy()
    {
        // 스크립트가 파괴될 때 구독 해제 (중요!)
        if (GameManager.instance != null)
            GameManager.instance.OnWeightIncreased -= StartCooldown;
    }
}
