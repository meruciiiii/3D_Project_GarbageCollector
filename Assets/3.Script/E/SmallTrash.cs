
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTrash : MonoBehaviour
{
    [SerializeField] private int trashNum;
    public int getTrashNum()
    {
        return trashNum;
    }
}