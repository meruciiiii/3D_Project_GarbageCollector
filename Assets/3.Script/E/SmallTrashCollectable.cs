
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTrashCollectable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerWork player)
    {
        Trash trash;
        if (!TryGetComponent<Trash>(out trash))
            return;
        if (!player.smallTrashAction.CanAdd(trash.Data.getSmallTrashWeight(trash.TrashNum)))
            return;

        player.smallTrashAction.Add(trash.Data.getSmallTrashWeight(trash.TrashNum));
        player.cleanPlayer.Clean(trash.Data.getSmallTrashHpdecrease(trash.TrashNum));
        //Debug.Log("trash.Data.getSmallTrashHpdecrease(trash.TrashNum) : " + trash.Data.getSmallTrashHpdecrease(trash.TrashNum));
        Destroy(gameObject);
    }
}