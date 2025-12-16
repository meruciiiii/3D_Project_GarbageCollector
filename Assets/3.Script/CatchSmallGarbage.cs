
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchSmallGarbage : MonoBehaviour
{
    //작은 쓰레기 일 경우 다음이 진행된다.
    //작은 쓰레기의 경우 해당 오브젝트가 사라진다.
    //작은 쓰레기의 경우 인벤토리에 들어간다.
    //인벤토리에 들어가면 인벤토리의 무게가 늘어난다.
    public GameObject trash;
    public void CatchTrash(GameObject trash)
    {
        this.trash = trash;
        AddBackpack();
        removeTrash();
    }
    public void removeTrash()
    {
        Destroy(trash);
    }
    public void AddBackpack()
    {
        // 쓰레기의 무게 데이터를 가져온다
        // 현재 가방 데이터를 가져온다
        // 현재 가방 무게에 쓰레기의 무게를 더한다
    }
}
