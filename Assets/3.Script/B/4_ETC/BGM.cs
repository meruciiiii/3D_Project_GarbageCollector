using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [SerializeField] private string BGMname;
    void Start()
    {
        AudioManager.instance.PlayBGM(BGMname);
    }
}
