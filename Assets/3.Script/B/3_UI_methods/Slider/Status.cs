using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public enum Statusenum
    {
        str,//0
        spd,//1
        amount//2
    }
    [SerializeField] private Statusenum s_enum;
    private Slider slider; 
    private void Awake()
    {
        transform.TryGetComponent(out slider);
    }

    private void OnEnable()
    {
        if (slider == null || GameManager.instance == null)
        {
            Debug.LogWarning("Status: Slider 또는 GameManager를 찾을 수 없습니다.");
            return;
        }

        switch (s_enum)
        {
            case Statusenum.str:
                    slider.maxValue = 7;
                    slider.value = GameManager.instance.P_Str;
                break;

            case Statusenum.spd :
                    slider.maxValue = 7;
                    slider.value = GameManager.instance.grab_speed;
                    break;

            case Statusenum.amount:
                    slider.maxValue = 7;
                    slider.value = GameManager.instance.grab_limit;
                break;

            default: 
                Debug.Log("status 예외 발생");
                break;
        }
    }
}
