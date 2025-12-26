
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTrashHoldable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerWork player)
    {
        Trash trash;
        if (!TryGetComponent<Trash>(out trash)) return;
        if (player.bigTrashAction.Hold(trash))
        {
            player.cleanPlayer.Clean(trash.Data.getBigTrashHpdecrease(trash.TrashNum));
        }
        //trash.gameObject.SetActive(false);
    }
}