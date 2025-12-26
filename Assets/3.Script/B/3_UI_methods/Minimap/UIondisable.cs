using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIondisable : MonoBehaviour
{
    [SerializeField] private GameObject UIvalue;
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject guide;
    [SerializeField] private GameObject aim;
    [SerializeField] private GameObject cross;

    // 이 오브젝트가 켜질 때 실행됨
    private void OnEnable()
    {
        if (UIvalue != null&&minimap!=null)
        {
            UIvalue.SetActive(false); // UI를 끔
            minimap.SetActive(false);
            guide.SetActive(false);
            aim.SetActive(false);
            cross.SetActive(false);
        }
    }

    // 이 오브젝트가 꺼질 때 실행됨
    private void OnDisable()
    {
        if (UIvalue != null)
        {
            UIvalue.SetActive(true); // UI를 킴
            minimap.SetActive(true);
            guide.SetActive(true);
            aim.SetActive(true);
            cross.SetActive(true);
        }
    }
}
