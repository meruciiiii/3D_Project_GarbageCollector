
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanPlayer : MonoBehaviour
{
    [SerializeField] private VignetteController vignetteController;
    [SerializeField] private PlayerIsDirty playerIsDirty;
    
    private int currentHP;
    private float cleanliness;
    private bool isTooDirty;
    private bool isDirty;

    public void Clean(int Hpdecrease)
    {
        currentHP = GameManager.instance.P_CurrentHP;
        currentHP -= Hpdecrease;
        cleanliness = (float)(currentHP * 100) / (float)GameManager.instance.P_MaxHP;
        isDirty = cleanliness <= 30;
        if (isDirty)
        {
            vignetteController.SetVignetteByCleanliness(cleanliness);
            playerIsDirty.CalDelay(cleanliness);
        }
        GameManager.instance.P_CurrentHP = currentHP;
    }
    
}
