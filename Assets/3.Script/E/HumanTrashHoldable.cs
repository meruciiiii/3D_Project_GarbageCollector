
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
}