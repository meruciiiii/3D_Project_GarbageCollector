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

    private void Awake()
    {
        input.onPickUp += StartCharging;
    }
    private void Start()
    {
        lastWeight = GameManager.instance.P_Weight;
    }

    public void StartCharging()
    {
        if (!gameObject.activeInHierarchy) return;
        if (chargeCoroutine != null) return;

        StartCoroutine(WaitAndCheckWeight());
    }

    private IEnumerator WaitAndCheckWeight()
    {
        // 1. 딱 한 프레임만 기다림 (모든 물리/데이터 업데이트가 끝날 때까지)
        // 만약 그래도 안 된다면 yield return new WaitForSeconds(0.05f); 정도로 수정 가능
        yield return null;

        int currentWeight = GameManager.instance.P_Weight;

        // 2. 이제 업데이트된 무게와 비교
        if (currentWeight > lastWeight)
        {
            // 성공 시에만 슬라이더 코루틴 시작
            chargeCoroutine = StartCoroutine(isGoing_co());
        }

        // 3. 늘어났든 줄어들었든 현재 무게를 무조건 동기화 (상점 판매 대응)
        lastWeight = currentWeight;
    }

    private IEnumerator isGoing_co()
    {
        cooldownSlider.value = 0; // 시작 시 초기화

        float duration = GameManager.instance.grab_speed;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            cooldownSlider.value = timer / duration; // 0에서 1까지 시간 비율로 채움
            yield return null;
        }

        cooldownSlider.value = cooldownSlider.maxValue;
        OnComplete();
    }

    private void OnComplete()
    {
        Debug.Log("줍기 준비 완료!");
        cooldownSlider.value = 0;
        chargeCoroutine = null;
    }

    private void OnEnable()
    {
        // 1. 다시 켜질 때 슬라이더 초기화
        cooldownSlider.value = 0;
        chargeCoroutine = null;

        // 2. 꺼져 있는 동안 변했을 무게 데이터를 최신화
        if (GameManager.instance != null)
        {
            lastWeight = GameManager.instance.P_Weight;
        }
    }

    private void OnDisable()
    {
        // 3. 오브젝트가 꺼지면 코루틴 변수 비우기 
        // (오브젝트가 꺼지는 순간 실행 중이던 코루틴은 유니티가 알아서 멈춥니다)
        chargeCoroutine = null;
    }
}
