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

    [Header("HP Slider Setting")]
    [SerializeField] private Image hpfill;//fill 할당
    private Color Hp_origin;
    [SerializeField] private Color hp_warning1 = new Color(0f,0f,0f);
    [SerializeField] private Color hp_warning2 = new Color(0f,0f,0f);

    [Header("Weight Slider Setting")]
    [SerializeField] private Image weightfill;//fill 할당
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

    private IEnumerator waitforvalue()
    {
        while(!GameManager.instance.LoadComplete) yield return null;
        UIManager.instance.change_Value();
    }
    
    public void moneyandweight()
    {
        float displayMoney = GameManager.instance.P_Money / 100.0f;
        moneytext.text = $"{displayMoney:F2}$";

        // 무게도 동일하게 100.0f로 나누어 소수점 두 자리를 표현합니다.
        float currentWeight = GameManager.instance.P_Weight / 100.0f;
        float maxWeight = GameManager.instance.P_Maxbag / 100.0f;

        weighttext.text = $"{currentWeight:F1} / {maxWeight:F1} kg";

        if (weightSlider != null)
        {
            weightSlider.maxValue = maxWeight;

            // 반대로 줄어드는 연출: (최대치 - 현재 사용량) = 남은 공간
            weightSlider.value = currentWeight;
            UpdateColorWeight(currentWeight, maxWeight);
        }
    }
    private void UpdateColorWeight(float current, float max)
    {
        if (weightfill == null) return;
        float average = (float)current / max;
        if (average > 0.75f) weightfill.color = weight_warning2;
        else if (average > 0.5f) weightfill.color = weight_warning1;
        else weightfill.color = weight_origin;
    }

    public void HP()
    {
        int currentHP = GameManager.instance.P_CurrentHP;
        int MaxHP = GameManager.instance.P_MaxHP;
        HPtext.text = $"{currentHP} / {MaxHP}";

        if (HPSlider != null)
        {
            HPSlider.maxValue = MaxHP; // 최대 체력 설정
            HPSlider.value = currentHP; // 현재 체력 설정
            UpdateColorHP(currentHP,MaxHP);
        }
    }

    private void UpdateColorHP(int current, int max)
    {
        if (hpfill == null) return;
        float average = (float)current / max;
        if (average < 0.25f) hpfill.color = hp_warning2;
        else if (average < 0.7f) hpfill.color = hp_warning1;
        else hpfill.color = Hp_origin;
    }
}
