using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetdata : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameManager.instance != null) GameManager.instance.ResetGameData();
    }
}
