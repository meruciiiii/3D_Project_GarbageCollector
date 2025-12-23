using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear : MonoBehaviour
{
    [SerializeField] private string clearSceneString;
    private Sceneload scene;
    private void Awake()
    {
        transform.TryGetComponent(out scene);
    }

    private void OnEnable()
    {
        scene.SceneLoader(clearSceneString);
    }
}
