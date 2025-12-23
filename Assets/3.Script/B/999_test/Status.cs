using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    [Header("슬라이더 할당")]
    [SerializeField] private Slider strSlider;
    [SerializeField] private Slider spdSlider;
    [SerializeField] private Slider amountSlider;

    private UIManager uimanager;

    private void Awake()
    {
        uimanager = FindAnyObjectByType<UIManager>();
    }

    private void Start()
    {
        if (uimanager != null)
        {
            uimanager.UIValueChange += UpdateAllStatusUI;
            // 초기값 반영
            UpdateAllStatusUI();
        }
    }

    private void OnDestroy()
    {
        if (uimanager != null)
        {
            uimanager.UIValueChange -= UpdateAllStatusUI;
        }
    }

    // 모든 슬라이더를 한 번에 갱신하는 함수
    private void UpdateAllStatusUI()
    {
        if (GameManager.instance == null) return;

        // 1. 힘(Str) 업데이트
        if (strSlider != null)
        {
            int baseStr = 1; // 기본값
            strSlider.wholeNumbers = true; // 코드로 정수 옵션 강제 활성화
            strSlider.minValue = 0;
            strSlider.maxValue = 6; // 1에서 7까지 총 6단계 업그레이드
            strSlider.value = GameManager.instance.P_Str - baseStr;
        }

        // 2. 속도(Spd) 업데이트
        if (spdSlider != null)
        {
            float startVal = 1.55f;
            float minVal = 0.25f;
            float currentVal = GameManager.instance.grab_speed;

            spdSlider.wholeNumbers = false; // 소수점 사용
            spdSlider.minValue = 0f;
            spdSlider.maxValue = 1f;
            float progress = (startVal - currentVal) / (startVal - minVal);
            spdSlider.value = Mathf.Clamp01(progress);
        }

        // 3. 수량(Amount) 업데이트
        if (amountSlider != null)
        {
            int baseAmount = 1;
            amountSlider.wholeNumbers = true; // 정수 옵션 활성화
            amountSlider.minValue = 0;
            amountSlider.maxValue = 4;
            amountSlider.value = GameManager.instance.grab_limit - baseAmount;
        }
    }
}
