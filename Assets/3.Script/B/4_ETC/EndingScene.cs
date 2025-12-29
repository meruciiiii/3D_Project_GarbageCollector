using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScene : MonoBehaviour
{
    [SerializeField] private string loadSceneString;
    [SerializeField] private float fallingsynctime = 3f;
    [SerializeField] private int load_wait_time = 17;
    private Sceneload scene;
    private void Awake()
    {
        transform.TryGetComponent(out scene);
    }

    IEnumerator Start()
    {
        AudioManager.instance.StopBGM();
        yield return new WaitForSeconds(fallingsynctime);
        //AudioManager.instance.PlaySFX(); ÈÖÀÌÀÌÀ× Åö ¶³¾îÁö´Â È¿°úÀ½
        yield return new WaitForSeconds(load_wait_time);
        scene.SceneLoader(loadSceneString);
    }
}
