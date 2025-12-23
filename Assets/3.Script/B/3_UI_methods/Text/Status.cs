using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    public enum Statusenum
    {
        str,
        spd,
        amount
    }
    [SerializeField] private Statusenum a;
    private Slider slider; 
    private void Awake()
    {
        transform.TryGetComponent(out slider);
    }

    private void OnEnable()
    {
        //switch(a)
    }
}
