using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private string BGMname;
    void Start()
    {
        if (BGMname == "BGM1" && GameManager.instance.GameClear) return;
        AudioManager.instance.PlayBGM(BGMname);
    }
}
