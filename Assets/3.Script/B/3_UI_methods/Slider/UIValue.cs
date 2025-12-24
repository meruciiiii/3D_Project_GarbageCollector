using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIValue : MonoBehaviour
{
    [SerializeField] private Text moneytext;
    [SerializeField] private Text weighttext;
    [SerializeField] private Text HPtext;
    [SerializeField] private Slider weightSlider;
    [SerializeField] private Slider HPSlider;

    private float currentDisplayMoney = 0f;//UI에 표시되고 있는 가짜돈
    private Coroutine moneycoroutine;
    private Coroutine weightCoroutine;
    private Coroutine hpCoroutine;

    [Header("HP Slider Setting")]
    [SerializeField] private Image hpfill; //슬라이더 fill 할당
    private Color Hp_origin;
    [SerializeField] private Color hp_warning1 = new Color(0f,0f,0f);
    [SerializeField] private Color hp_warning2 = new Color(0f,0f,0f);

    [Header("Weight Slider Setting")]
    [SerializeField] private Image weightfill;
    private Color weight_origin;
    [SerializeField] private Color weight_warning1 = new Color(0f, 0f, 0f);
    [SerializeField] private Color weight_warning2 = new Color(0f, 0f, 0f);

    private void Awake()
    {
        if (hpfill != null) Hp_origin = hpfill.color;
        if (weightfill != null) weight_origin = weightfill.color;
    }

    private void Start()
    {
        UIManager uimanager = FindFirstObjectByType<UIManager>();
        uimanager.UIValueChange += moneyandweight;
        uimanager.UIValueChange += HP;

        StartCoroutine(waitforvalue());
    }

    private void OnEnable()
    {
        moneyandweight();
        HP();
    }

    private IEnumerator waitforvalue()
    {
        while(!GameManager.instance.LoadComplete) yield return null;

        currentDisplayMoney = GameManager.instance.P_Money / 100.0f;
        moneytext.text = $"{currentDisplayMoney:F2}$";

        UIManager.instance.change_Value();
    }
    
    public void moneyandweight()
    {
        if (GameManager.instance == null || !GameManager.instance.LoadComplete) return;
        if (!gameObject.activeInHierarchy) return;
        float targetMoney = GameManager.instance.P_Money / 100.0f;

        if (moneycoroutine != null) StopCoroutine(moneycoroutine);
        moneycoroutine = StartCoroutine(AnimateMoney(targetMoney));

        // 무게도 동일하게 100.0f로 나누어 소수점 두 자리를 표현
        float targetWeight = GameManager.instance.P_Weight / 100.0f;
        float maxWeight = GameManager.instance.P_Maxbag / 100.0f;

        if (weightSlider != null) weightSlider.maxValue = maxWeight;

        if (weightCoroutine != null) StopCoroutine(weightCoroutine);
        weightCoroutine = StartCoroutine(AnimateWeight(targetWeight, maxWeight));
    }
    public void HP()
    {
        if (GameManager.instance == null || !GameManager.instance.LoadComplete) return;
        if (!gameObject.activeInHierarchy) return;
        int targetHP = GameManager.instance.P_CurrentHP;
        int maxHP = GameManager.instance.P_MaxHP;

        if (HPSlider != null) HPSlider.maxValue = maxHP;

        if (hpCoroutine != null) StopCoroutine(hpCoroutine);
        hpCoroutine = StartCoroutine(AnimateHP(targetHP, maxHP));
    }

    private void UpdateColorWeight(float current, float max)
    {
        if (weightfill == null) return;
        float average = (float)current / max;
        if (average > 0.75f) weightfill.color = weight_warning2;
        else if (average > 0.5f) weightfill.color = weight_warning1;
        else weightfill.color = weight_origin;
    }

    private void UpdateColorHP(int current, int max)
    {
        if (hpfill == null) return;
        float average = (float)current / max;
        if (average < 0.25f) hpfill.color = hp_warning2;
        else if (average < 0.7f) hpfill.color = hp_warning1;
        else hpfill.color = Hp_origin;
    }
    private IEnumerator AnimateMoney(float target)
    {
        float duration = 0.5f; // 0.5초 동안 올라감, 사운드 길이에 따라 바꿔주세요
        float elapsed = 0f;
        float startValue = currentDisplayMoney; // 현재 화면에 찍힌 값에서 시작

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Lerp를 사용하여 부드럽게 값 증가
            currentDisplayMoney = Mathf.Lerp(startValue, target, elapsed / duration);
            moneytext.text = $"{currentDisplayMoney:N2}";
            yield return null;
        }

        // 마지막 오차 보정
        currentDisplayMoney = target;
        moneytext.text = $"{currentDisplayMoney:N2}";
    }

    private IEnumerator AnimateWeight(float target, float max)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        float startValue = weightSlider.value;// 현재 슬라이더 값에서 시작

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, target, elapsed / duration);

            weighttext.text = $"{currentValue:F1} / {max:F1} kg";
            weightSlider.value = currentValue;
            UpdateColorWeight(currentValue, max);
            yield return null;
        }

        weightSlider.value = target;
        weighttext.text = $"{target:F1} / {max:F1} kg";
        UpdateColorWeight(target, max);
    }

    private IEnumerator AnimateHP(int target, int max)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        float startValue = HPSlider.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 체력은 보통 정수로 보이므로 float로 계산 후 int로 캐스팅
            float currentValue = Mathf.Lerp(startValue, (float)target, elapsed / duration);

            HPtext.text = $"{(int)currentValue} / {max}";
            HPSlider.value = currentValue;
            UpdateColorHP((int)currentValue, max);
            yield return null;
        }

        HPSlider.value = target;
        HPtext.text = $"{target} / {max}";
        UpdateColorHP(target, max);
    }
}
