
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmallTrash : MonoBehaviour
{
    [SerializeField] private int smallTrashNum;
    public int getTrashNum()
    {
        return smallTrashNum;
    }

    [SerializeField] private Material outline_material;
    private Renderer render;

    private void Start() {
        TryGetComponent(out render);
    }

    public void onOutline() {
        List<Material> materials = render.sharedMaterials.ToList();
        materials.Add(outline_material);
        render.materials = materials.ToArray();
    }

    public void offOutline() {
        List<Material> materials = render.sharedMaterials.ToList();
        materials.Remove(outline_material);
        render.materials = materials.ToArray();
    }
}