
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTrashAction : MonoBehaviour
{
    private Trash currentTrash;
    private bool isHolding => currentTrash != null;
    public bool IsHolding => isHolding;
    //public void TryInteractWithBigTrash(Trash trash)
    //{
    //    if (IsHolding)
    //    {
    //        DrobGarbage(trash);
    //    }
    //    else
    //    {
    //        Hold(trash);
    //    }
    //}
    public bool Hold(Trash trash)
    {
        if (CanHold(trash.Data.getrequiredStrength(trash.TrashNum)))
            return false;
        currentTrash = trash;
        //Debug.Log(transform.GetChild(trash.TrashNum + 1).gameObject.name+" 활성화?" +transform.GetChild(trash.TrashNum + 1).gameObject.activeSelf);
        transform.GetChild(trash.TrashNum + 1).gameObject.SetActive(true);
        //Debug.Log(transform.GetChild(trash.TrashNum + 1).gameObject.name+" 활성화?" +transform.GetChild(trash.TrashNum + 1).gameObject.activeSelf);
        GameManager.instance.BigGarbageWeight = trash.Data.getBigTrashWeight(trash.TrashNum);
        currentTrash.Trash_c.enabled = false;
        currentTrash.Trash_render.enabled = false;
        foreach (var children_r in currentTrash.GetComponentsInChildren<Renderer>(true))
        {
            children_r.enabled = false;
        }
        GameManager.instance.isGrabBigGarbage = true;
        AudioManager.instance.PlaySFX("SFX7");//큰 쓰레기 소리 작업
        return true;
    }
    public bool CanHold(int strength)
    {
        if (GameManager.instance.P_Str < strength) 
            return true; //들 수 없다.
        else
        {
            GameManager.instance.BigneedStr = strength;
            return false; //들 수 있다.
        }
    }
    public void DrobGarbage()                                                                //수정해야 할 것
    {
        Trash trash = currentTrash;
        //Debug.Log("is it Droping");
        Transform trashRotation = trash.transform;                                                                //trash
        trashRotation.rotation = transform.GetChild(trash.TrashNum + 1).gameObject.transform.rotation;
        transform.GetChild(trash.TrashNum + 1).gameObject.SetActive(false);
        Vector3 direction = (transform.forward + transform.right * 0.5f).normalized;
        trashRotation.position = transform.position + direction * 1.5f;
        trashRotation.rotation = transform.rotation;
        //trash.gameObject.SetActive(true);                                                                //수정해야 할 것
        foreach (var children_r in trash.GetComponentsInChildren<Renderer>(true))
        {
            children_r.enabled = true;
        }
        trash.Trash_r.isKinematic = false;
        trash.Trash_r.useGravity = true;
        trash.Trash_r.AddForce(direction * 1f, ForceMode.Impulse);
        trash.Trash_r.AddTorque(Vector3.Cross(Vector3.up, direction) * 1f);
        //Debug.Log(transform.position);
        GameManager.instance.isGrabBigGarbage = false;
        currentTrash.Trash_c.enabled = true;
        currentTrash.Trash_render.enabled = true;
        currentTrash = null;
    }
    public void SellHeldTrash()
    {
        if (currentTrash == null)
            return;
        transform.GetChild(currentTrash.TrashNum + 1).gameObject.SetActive(false);
        Destroy(currentTrash.gameObject);
        currentTrash = null;
    }
}
