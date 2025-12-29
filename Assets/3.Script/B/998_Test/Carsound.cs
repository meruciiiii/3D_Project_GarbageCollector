using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carsound : MonoBehaviour
{
    [SerializeField] private string SFX3D_sound_name = "SFX3D2";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.Play3DSFX(SFX3D_sound_name, this.transform);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        AudioManager.instance.Stop3DSFX(SFX3D_sound_name, this.transform);
    //    }
    //}
    //
    //private void OnDisable() // 트리거 오브젝트가 꺼지거나 씬에서 사라질 때 호출
    //{
    //    if (AudioManager.instance != null)
    //    {
    //        StopSound();
    //    }
    //}
    //
    //private void StopSound()
    //{
    //    AudioManager.instance.Stop3DSFX(SFX3D_sound_name, this.transform);
    //}
}
