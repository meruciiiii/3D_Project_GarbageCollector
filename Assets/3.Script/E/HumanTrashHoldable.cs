
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTrashHoldable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerWork player)
    {
        Trash trash;
        if (!TryGetComponent<Trash>(out trash))
            return;
        player.humanTrashAction.Hold(trash);
        player.cleanPlayer.Clean(trash.Data.getBigTrashHpdecrease(trash.TrashNum));
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