using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaTextUI : MonoBehaviour
{
    [SerializeField] private Text AreaText;

    private void OnEnable()
    {
        if(AreaManager.instance != null)
        {
            AreaManager.instance.onAreaChanged += UpdaeAreaText;
        }
    }

    private void Start()
    {
        if (GameManager.instance != null)
        {
            UpdaeAreaText(GameManager.instance.Current_Area);
        }
    }

    private void OnDisable()
    {
        if(AreaManager.instance != null)
        {
            AreaManager.instance.onAreaChanged -= UpdaeAreaText;
        }
    }

    private void UpdaeAreaText(int area)
    {
        if (AreaText == null) return;

        string areaname = "";
        bool isEnglish = GameManager.instance.P_isEnglish;

        switch (area)
        {
            case 1:
                if (isEnglish) areaname = "1EN";
                else areaname = "1KR";
                break;
            case 2:
                if (isEnglish) areaname = "2EN";
                else areaname = "2KR";
                break;
            case 3:
                if (isEnglish) areaname = "3EN";
                else areaname = "3KR";
                break;
            default:
                if (isEnglish) areaname = "Unknown Area";
                else areaname = "알 수 없는 구역";
                break;
        }
        AreaText.text = $"{areaname}";
    }
}
