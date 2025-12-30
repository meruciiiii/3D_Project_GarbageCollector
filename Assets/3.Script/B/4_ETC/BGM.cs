using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private string BGMname;
    void Start()
    {
        if (BGMname == "BGM1" && GameManager.instance.GameClear) 
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.SetBGMloop(false);
            }
            return; 
        }
        AudioManager.instance.PlayBGM(BGMname);
    }
}
