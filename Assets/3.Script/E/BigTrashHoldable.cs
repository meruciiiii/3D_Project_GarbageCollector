
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
    public int TrashWeight()
    {
        Trash trash;
        if (!TryGetComponent<Trash>(out trash))
            return -1;
        return trash.Data.getBigTrashWeight(trash.TrashNum);
    }
    public int TrashStr()
    {
        Trash trash;
        if (!TryGetComponent<Trash>(out trash))
            return -1;
        return trash.Data.getrequiredStrength(trash.TrashNum);
    }
}