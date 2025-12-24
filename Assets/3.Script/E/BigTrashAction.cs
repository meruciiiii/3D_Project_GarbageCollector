
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTrashAction : MonoBehaviour
{
    private Trash currentTrash;
    public void TryInteractWithBigTrash(Trash trash)
    {
        if (!GameManager.instance.isGrabBigGarbage)
        {
            DrobGarbage(trash);
        }
        else
        {
            Hold(trash);
        }
    }
    public void Hold(Trash trash)
    {
        if (CanHold(trash.Data.getrequiredStrength(trash.TrashNum)))
            return;
        currentTrash = trash;
        transform.GetChild(trash.TrashNum + 1).gameObject.SetActive(true);
        GameManager.instance.P_Weight += GameManager.instance.P_Weight + trash.Data.getSmallTrashWeight(trash.TrashNum);
    }
    public bool CanHold(int strength)
    {
        if (GameManager.instance.P_Str >= strength)
            return true; //들 수 있다.
        else
            return false; //들 수 없다.
    }
    public void DrobGarbage(Trash trash)                                                                //수정해야 할 것
    {
        Transform trashRotation = trash.transform;                                                                //trash
        trashRotation.rotation = transform.GetChild(trash.TrashNum + 1).gameObject.transform.rotation;
        transform.GetChild(trash.TrashNum + 1).gameObject.SetActive(false);
        Vector3 direction = (transform.forward + transform.right * 0.5f).normalized;
        trashRotation.position = transform.position + direction * 1.5f;
        trashRotation.rotation = transform.rotation;
        //trash.gameObject.SetActive(true);                                                                //수정해야 할 것
        trash.Trash_r.isKinematic = false;
        trash.Trash_r.useGravity = true;
        trash.Trash_r.AddForce(direction * 1f, ForceMode.Impulse);
        trash.Trash_r.AddTorque(Vector3.Cross(Vector3.up, direction) * 1f);
        //Debug.Log(transform.position);
        GameManager.instance.isGrabBigGarbage = false;
    }
    public void SellHeldTrash()
    {
        if (currentTrash == null)
            return;
        transform.GetChild(currentTrash.TrashNum + 1).gameObject.SetActive(false);
        Destroy(currentTrash.gameObject);
        currentTrash = null;
        // 손에 들고 있던 비주얼 끄기
        // 돈 지급
    }
}
