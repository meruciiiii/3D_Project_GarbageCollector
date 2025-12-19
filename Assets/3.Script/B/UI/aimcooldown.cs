using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class aimcooldown : MonoBehaviour
{
    [SerializeField] private Slider cooldownSlider;
    private Coroutine chargeCourutine;

    public void StartCharging()
    {
        if (chargeCourutine != null) StopCoroutine(chargeCourutine);
        //chargeCourutine = StartCoroutine();
    }
    
}
