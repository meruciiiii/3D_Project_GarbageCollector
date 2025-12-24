
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTrashHoldable : MonoBehaviour, IInteractable
{
    public void Interact(PlayerWork player)
    {
        Trash trash;
        TryGetComponent<Trash>(out trash);

        

        player.bigTrashAction.TryInteractWithBigTrash(trash);
        player.cleanPlayer.Clean(trash.Data.getBigTrashHpdecrease(trash.TrashNum));
        //trash.gameObject.SetActive(false);
    }
}
