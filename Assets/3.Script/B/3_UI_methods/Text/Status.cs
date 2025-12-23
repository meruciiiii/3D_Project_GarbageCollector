using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    [Header("Ç¥½Ã °ª 0 = str, 1= spd, 2 Amount")]
    [SerializeField] private int num;
    private Slider slider; 
    private void Awake()
    {
        transform.TryGetComponent(out slider);
    }

    private void OnEnable()
    {
        
    }
}
