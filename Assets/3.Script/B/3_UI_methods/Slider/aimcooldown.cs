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
        if (!gameObject.activeInHierarchy) return;
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
    private void ResetSlider()
    {
        if (cooldownSlider != null)
        {
            cooldownSlider.value = 0f;
        }
    }
    private void OnEnable()
    {
        ResetSlider();
    }

    // 오브젝트가 비활성화될 때 호출 (에임이 꺼질 때)
    private void OnDisable()
    {
        // 꺼질 때 코루틴 변수 참조를 정리하고 슬라이더를 초기화합니다.
        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
        }
        ResetSlider();
    }

    private void OnDestroy()
    {
        // 스크립트가 파괴될 때 구독 해제 (중요!)
        if (GameManager.instance != null)
            GameManager.instance.OnWeightIncreased -= StartCooldown;
    }
}
