using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyandWeightText : MonoBehaviour
{
    [SerializeField] private Text moneytext;
    [SerializeField] private Text weighttext;
    private void Update()
    {
        float displayMoney = GameManager.instance.P_Money / 100.0f;
        moneytext.text = $"Money : {displayMoney:F2} dollar";

        // 무게도 동일하게 100.0f로 나누어 소수점 두 자리를 표현합니다.
        float currentWeight = GameManager.instance.P_Weight / 100.0f;
        float maxWeight = GameManager.instance.P_Maxbag / 100.0f;

        weighttext.text = $"Weight : {currentWeight:F2} / {maxWeight:F2} kg";
    }
}
