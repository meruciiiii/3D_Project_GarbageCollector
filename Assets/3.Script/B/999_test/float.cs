using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floats : MonoBehaviour
{
    private float maxweight = 6f;
    private float weight = 0f;
    private float a_weight = 0.1f;
    private float b_weight = 5f;

    private void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            weight += a_weight;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (maxweight >= weight + b_weight)
                weight += b_weight;
            else Debug.Log("¾ÈµÊ");

        }
        Debug.Log(weight);
    }

}
