using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetdata : MonoBehaviour
{
    private void OnEnable()
    {
        while (GameManager.instance == null) continue;
        GameManager.instance.ResetGameData();
    }
}
