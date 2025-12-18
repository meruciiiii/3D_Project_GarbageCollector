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
        moneytext.text = $"Money : {displayMoney:F2} dollar";

        // 무게도 동일하게 100.0f로 나누어 소수점 두 자리를 표현합니다.
        float currentWeight = GameManager.instance.P_Weight / 100.0f;
        float maxWeight = GameManager.instance.P_Maxbag / 100.0f;

        weighttext.text = $"Weight : {currentWeight:F1} / {maxWeight:F1} kg";
    }

    public void money()
    {
        float displayMoney = GameManager.instance.P_Money / 100.0f;
        moneytext.text = $"Money : {displayMoney:F2} dollar";
    }

    public void weight()
    {
        float currentWeight = GameManager.instance.P_Weight / 100.0f;
        float maxWeight = GameManager.instance.P_Maxbag / 100.0f;

        weighttext.text = $"Weight : {currentWeight:F1} / {maxWeight:F1} kg";
    }

    public void HP()
    {
        int currentHP = GameManager.instance.P_CurrentHP;
        int MaxHP = GameManager.instance.P_MaxHP;
        HPtext.text = $"HP : {currentHP} / {MaxHP}";
    }
}
