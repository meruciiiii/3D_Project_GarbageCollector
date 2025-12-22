using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.5f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
