
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public enum TrashSize
    {
        Small,
        Large,
        Human
    }
    [SerializeField] private TrashSize size;
    [SerializeField] private int trashNum;
    [SerializeField] private TrashData trashData;
    [SerializeField] private Rigidbody trash_r;
    [SerializeField] private Collider trash_c;
    [SerializeField] private Renderer trash_render;

    public TrashSize Size => size;
    public int TrashNum => trashNum;
    public TrashData Data => trashData;
    public Rigidbody Trash_r => trash_r;
    public Collider Trash_c => trash_c;
    public Renderer Trash_render => trash_render;
}