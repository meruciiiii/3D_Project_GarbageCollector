using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheat : MonoBehaviour
{
    public void Cheat()
    {
        if (GameManager.instance != null) GameManager.instance.Cheatdata();
    }
}
