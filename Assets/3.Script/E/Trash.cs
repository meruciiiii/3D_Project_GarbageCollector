
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public enum TrashSize
    {
        Small,
        Large
    }
    [SerializeField] private TrashSize size;
    [SerializeField] private int trashNum;
    [SerializeField] private TrashData trashData;
    [SerializeField] private Rigidbody trash_r;

    public TrashSize Size => size;
    public int TrashNum => trashNum;
    public TrashData Data => trashData;
    public Rigidbody Trash_r => trash_r;
}