using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aimcooldown : MonoBehaviour
{
    [SerializeField] private Slider cooldownSlider;
    [SerializeField] private PlayerInput input;

    private Coroutine chargeCoroutine;
    private int lastWeight = 0;
    private bool isPicking = false;

    private void Awake()
    {
        // 입력 이벤트 연결 (누르고 있는지 여부만 판단)
        input.onPickUp += () => isPicking = true;
        input.offPickUp += () => { isPicking = false; ResetSlider(); };
    }

    private void Start()
    {
        // 시작 시 현재 무게 저장
        if (GameManager.instance != null)
            lastWeight = GameManager.instance.P_Weight;
    }

    private void Update()
    {
        if (GameManager.instance == null) return;

        int currentWeight = GameManager.instance.P_Weight;

        // 1. 무게가 늘어났는지 감시 (쓰레기 줍기 성공 감지)
        if (currentWeight > lastWeight)
        {
            // 줍기 성공 시 슬라이더 코루틴 시작
            if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
            chargeCoroutine = StartCoroutine(CooldownRoutine());

            // 무게 값 동기화
            lastWeight = currentWeight;
        }
        // 2. 무게가 줄어들었을 때 (정산 등) 값 최신화
        else if (currentWeight < lastWeight)
        {
            lastWeight = currentWeight;
        }
    }

    private IEnumerator CooldownRoutine()
    {
        // 0에서 100(1.0f)까지 채우기
        float duration = GameManager.instance.grab_speed;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            cooldownSlider.value = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        // 100% 도달 완료
        cooldownSlider.value = 1f;

        // 3. 완료 직후 0으로 초기화 (원하시는 로직)
        yield return new WaitForEndOfFrame();
        cooldownSlider.value = 0;
        chargeCoroutine = null;
    }

    private void ResetSlider()
    {
        if (chargeCoroutine != null) return; // 코루틴 도중에는 0으로 밀지 않음
        cooldownSlider.value = 0;
    }

    private void OnEnable()
    {
        if (GameManager.instance != null) lastWeight = GameManager.instance.P_Weight;
        cooldownSlider.value = 0;
    }
}
